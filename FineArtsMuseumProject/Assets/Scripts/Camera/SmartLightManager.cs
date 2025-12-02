// Scripts/Runtime/SmartLightManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SmartLightManager : MonoBehaviour
{
    [Header("Core settings")]
    public UnityEngine.Camera targetCamera;                  // nếu null -> Camera.main
    public Light[] candidateLights;              // nếu null hoặc empty sẽ tìm tất cả Light trong scene (không include Directional)
    public LayerMask occlusionMask = ~0;         // Layer mask cho raycast (what can occlude)
    public float updateInterval = 0.15f;         // how often we recompute relevance
    public int maxActiveLights = 8;              // số lượng ánh sáng tối đa bật đồng thời
    public float maxDistance = 20f;              // đèn ngoài khoảng này được ignore
    [Range(0f, 179f)] public float coneHalfAngle = 90f; // half-angle (deg) of camera cone
    public float fadeSpeed = 6f;                 // speed of intensity lerp (higher -> faster)
    public bool disableWhenZero = true;
    public bool disableGameObjectWhenZero = false;
    public int maxShadowedLights = 1;            // chỉ cho phép K nearest lights có shadows enabled

    [Header("Fallback (fill-up) settings")]
    public float fallbackRadius = 10f;           // bán kính xung quanh camera để tìm lights bổ sung
    [Range(0f,1f)] public float fallbackIntensityScale = 0.5f; // scale intensity cho fallback lights (0..1)
    public bool fallbackUseDistanceFalloff = true; // nếu true: scale = fallbackIntensityScale * (1 - dist/fallbackRadius)
    public bool fallbackIgnoreCone = true;       // Nếu true -> fallback dựa trên radius bất kể hướng camera

    [Header("Advanced / Performance")]
    public bool useSphereCheckForOcclusion = false;
    public float occlusionSphereRadius = 0.05f;
    public float nearClipDistance = 0.5f;

    // internal
    struct LightEntry
    {
        public Light light;
        public float originalIntensity;
        public float currentTargetIntensity;
        public bool isRelevant;
        public Vector3 position;
        public int index;
    }

    private LightEntry[] entries;
    private int entriesCount = 0;
    private float[] tempDistances; // reuse for sorting if needed
    private UnityEngine.Camera camCache;

    // helper lists (reuse to reduce allocations)
    private List<int> relevantIndices = new List<int>(64);
    private List<int> fallbackCandidates = new List<int>(64);

    void Awake()
    {
        if (targetCamera == null) targetCamera = UnityEngine.Camera.main;
        camCache = targetCamera;
        InitCandidates();
    }

    void OnValidate()
    {
        if (updateInterval <= 0.01f) updateInterval = 0.01f;
        if (maxActiveLights < 1) maxActiveLights = 1;
        if (maxShadowedLights < 0) maxShadowedLights = 0;
        if (fallbackRadius < 0f) fallbackRadius = 0f;
        if (fallbackIntensityScale < 0f) fallbackIntensityScale = 0f;
    }

    void InitCandidates()
    {
        if (candidateLights == null || candidateLights.Length == 0)
        {
            var all = GameObject.FindObjectsOfType<Light>();
            var list = new List<Light>(all.Length);
            foreach (var l in all)
            {
                if (l == null) continue;
                if (l.type == LightType.Directional) continue;
                list.Add(l);
            }
            candidateLights = list.ToArray();
        }

        entriesCount = candidateLights != null ? candidateLights.Length : 0;
        entries = new LightEntry[entriesCount];
        tempDistances = new float[Math.Max(1, entriesCount)];

        for (int i = 0; i < entriesCount; i++)
        {
            var L = candidateLights[i];
            if (L == null) continue;
            entries[i].light = L;
            entries[i].originalIntensity = L.intensity;
            entries[i].currentTargetIntensity = 0f;
            entries[i].isRelevant = false;
            entries[i].position = L.transform.position;
            entries[i].index = i;
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator UpdateLoop()
    {
        var wait = new WaitForSeconds(updateInterval);
        while (true)
        {
            RecomputeRelevance();
            yield return wait;
        }
    }

    void RecomputeRelevance()
    {
        if (camCache == null)
        {
            if (targetCamera != null) camCache = targetCamera;
            else return;
        }

        relevantIndices.Clear();
        fallbackCandidates.Clear();

        Vector3 camPos = camCache.transform.position;
        Vector3 camForward = camCache.transform.forward;
        float cosThreshold = Mathf.Cos(Mathf.Deg2Rad * coneHalfAngle);

        // 1) Cull by distance/cone/occlusion -> mark relevant; collect fallback candidates based on distance only (if fallbackIgnoreCone true)
        for (int i = 0; i < entriesCount; i++)
        {
            var L = entries[i].light;
            if (L == null)
            {
                entries[i].isRelevant = false;
                tempDistances[i] = float.MaxValue;
                continue;
            }

            Vector3 pos = L.transform.position;
            entries[i].position = pos;
            float dist = Vector3.Distance(camPos, pos);
            tempDistances[i] = dist;

            // base distance cutoff for fully ignoring far lights
            if (dist > maxDistance)
            {
                entries[i].isRelevant = false;
                continue;
            }

            // cone test - do NOT 'continue' here, we just mark whether it passed
            Vector3 toLight = (pos - camPos).normalized;
            bool passedCone = Vector3.Dot(camForward, toLight) >= cosThreshold;

            // occlusion check only if it passed cone (optional optimization)
            bool occluded = false;
            if (passedCone && dist > nearClipDistance)
            {
                Ray r = new Ray(camPos, pos - camPos);
                RaycastHit hit;
                float maxDist = dist - 0.05f;
                if (useSphereCheckForOcclusion)
                {
                    if (Physics.SphereCast(r, occlusionSphereRadius, out hit, maxDist, occlusionMask.value, QueryTriggerInteraction.Ignore))
                        occluded = true;
                }
                else
                {
                    if (Physics.Raycast(r, out hit, maxDist, occlusionMask.value, QueryTriggerInteraction.Ignore))
                        occluded = true;
                }
            }

            // isRelevant only if passed cone AND not occluded
            entries[i].isRelevant = (passedCone && !occluded);
            if (entries[i].isRelevant)
            {
                relevantIndices.Add(i);
            }

            // collect fallback candidates based on distance (and optionally ignore cone)
            // If fallbackIgnoreCone==true -> any non-relevant light within fallbackRadius qualifies
            // If false -> require it to have passedCone to be a fallback candidate (i.e. behave like before)
            if (fallbackRadius > 0f && dist <= fallbackRadius && dist <= maxDistance)
            {
                if (fallbackIgnoreCone)
                {
                    // allow even if behind camera
                    if (!entries[i].isRelevant)
                        fallbackCandidates.Add(i);
                }
                else
                {
                    // only allow fallback if it at least passed cone (so still direction-based)
                    if (!entries[i].isRelevant && passedCone)
                        fallbackCandidates.Add(i);
                }
            }
        }

        // 2) If too many relevant, sort by distance and trim to maxActiveLights
        if (relevantIndices.Count > maxActiveLights)
        {
            relevantIndices.Sort((a, b) => tempDistances[a].CompareTo(tempDistances[b]));
            // keep only first maxActiveLights in active stage
        }

        // 3) Determine active set: prioritizedRelevant (nearest up to maxActiveLights)
        var activeSet = new HashSet<int>();
        relevantIndices.Sort((a, b) => tempDistances[a].CompareTo(tempDistances[b])); // ensure sorted
        int activeCount = 0;
        for (int i = 0; i < relevantIndices.Count && activeCount < maxActiveLights; i++)
        {
            activeSet.Add(relevantIndices[i]);
            activeCount++;
        }

        // 4) If activeCount < maxActiveLights, fill from fallbackCandidates (nearest first)
        if (activeCount < maxActiveLights && fallbackCandidates.Count > 0)
        {
            fallbackCandidates.Sort((a, b) => tempDistances[a].CompareTo(tempDistances[b]));
            for (int i = 0; i < fallbackCandidates.Count && activeCount < maxActiveLights; i++)
            {
                int idx = fallbackCandidates[i];
                if (!activeSet.Contains(idx))
                {
                    activeSet.Add(idx);
                    activeCount++;
                }
            }
        }

        // 5) Assign target intensities
        for (int i = 0; i < entriesCount; i++)
        {
            entries[i].currentTargetIntensity = 0f; // default off
        }

        var activeList = new List<int>(activeSet.Count);
        foreach (var idx in activeSet) activeList.Add(idx);
        activeList.Sort((a, b) => tempDistances[a].CompareTo(tempDistances[b]));

        int shadowAssigned = 0;
        for (int rank = 0; rank < activeList.Count; rank++)
        {
            int idx = activeList[rank];
            var L = entries[idx].light;
            if (L == null) continue;

            bool wasRelevant = entries[idx].isRelevant;
            float dist = tempDistances[idx];

            float baseInt = entries[idx].originalIntensity;
            float target = 0f;
            if (wasRelevant)
            {
                target = baseInt;
            }
            else
            {
                if (fallbackUseDistanceFalloff && fallbackRadius > 0f)
                {
                    float fall = 1f - Mathf.Clamp01(dist / fallbackRadius);
                    target = baseInt * Mathf.Clamp01(fallbackIntensityScale * fall);
                }
                else
                {
                    target = baseInt * fallbackIntensityScale;
                }
            }

            entries[idx].currentTargetIntensity = target;

            if (shadowAssigned < maxShadowedLights && target > 0.001f)
            {
                if (L.shadows == LightShadows.None)
                    L.shadows = LightShadows.Soft;
                shadowAssigned++;
            }
            else
            {
                if (L.shadows != LightShadows.None)
                    L.shadows = LightShadows.None;
            }
        }

        // For any non-active lights not in activeSet: ensure they are target 0 and shadows off
        for (int i = 0; i < entriesCount; i++)
        {
            if (!activeSet.Contains(i))
            {
                entries[i].currentTargetIntensity = 0f;
                var L = entries[i].light;
                if (L != null && L.shadows != LightShadows.None)
                    L.shadows = LightShadows.None;
            }
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < entriesCount; i++)
        {
            var e = entries[i];
            var L = e.light;
            if (L == null) continue;

            float current = L.intensity;
            float target = e.currentTargetIntensity;
            if (!Mathf.Approximately(current, target))
            {
                float speed = fadeSpeed * Mathf.Max(1f, e.originalIntensity);
                float next = Mathf.MoveTowards(current, target, speed * dt);
                L.intensity = next;

                if (disableWhenZero)
                {
                    if (next <= 0.001f && target == 0f)
                    {
                        L.enabled = false;
                        if (disableGameObjectWhenZero) L.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (!L.enabled) L.enabled = true;
                        if (disableGameObjectWhenZero && !L.gameObject.activeSelf) L.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    #region Public API (helper)
    public void SetCandidateLights(Light[] lights)
    {
        candidateLights = lights;
        InitCandidates();
    }

    public void AddCandidateLight(Light l)
    {
        if (l == null) return;
        var list = new List<Light>(candidateLights ?? Array.Empty<Light>());
        if (!list.Contains(l))
        {
            list.Add(l);
            candidateLights = list.ToArray();
            InitCandidates();
        }
    }

    public void RemoveCandidateLight(Light l)
    {
        if (candidateLights == null || l == null) return;
        var list = new List<Light>(candidateLights);
        if (list.Remove(l))
        {
            candidateLights = list.ToArray();
            InitCandidates();
        }
    }
    #endregion
}
