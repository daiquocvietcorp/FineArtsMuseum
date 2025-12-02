using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Video;


    public class AIVideoManager : MonoSingleton<AIVideoManager>
    {
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        [field: SerializeField] private List<AIVideo> _aiVideos;
        [field: SerializeField] private Material videoMaterial;
        [field: SerializeField] private Material transparentVideoMaterial;
        [field: SerializeField] private VideoPlayer videoPlayer;
        [field: SerializeField] private RenderTexture videoRenderTexture;
        [field: SerializeField] private GameObject blinkCanvas;
        
        private Dictionary<string, AIVideo> _aiVideoPlayers;
        private Coroutine _aiCoroutine;
        private const int MinWaitTime = 2;
        
        private void Awake()
        {
            if (_aiVideos == null)
                return;

            _aiVideoPlayers = new Dictionary<string, AIVideo>();
            if(_aiVideoPlayers == null) return;
            
            foreach (var aiVideo in _aiVideos)
            {
                _aiVideoPlayers.Add(aiVideo.AntiqueObjectName, aiVideo);
            }
        }
        
        public bool IsObjectHasVideo(string objectName)
        {
            return _aiVideoPlayers.ContainsKey(objectName);
        }

        public void PreSetVideo(string objectName)
        {
            if(!_aiVideoPlayers.TryGetValue(objectName, out var aiVideo)) return;
            if(aiVideo.MeshRenderer == null || aiVideo.videoClip == null || aiVideo.originalMaterial == null) return;
            videoPlayer.clip = aiVideo.videoClip;
            if(aiVideo.IsPlaneReplace) aiVideo.MeshRenderer.gameObject.SetActive(false);
        }

        public void SetVideoMaterial(string objectName)
        {
            if(!_aiVideoPlayers.TryGetValue(objectName, out var aiVideo)) return;
            if(aiVideo.MeshRenderer == null || aiVideo.videoClip == null || aiVideo.originalMaterial == null) return;
            if(_aiCoroutine != null) StopCoroutine(_aiCoroutine);
            _aiCoroutine = StartCoroutine(StartAIVideoCoroutine(aiVideo));
            
            //materials[materialIndex].SetTexture(EmissionMap, videoRenderTexture);
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.K))
            // {
            //     Time.timeScale = 0.1f;
            // }
            // if (Input.GetKeyDown(KeyCode.L))
            // {
            //     Time.timeScale = 1f;
            // }
        }

        private IEnumerator StartAIVideoCoroutine(AIVideo aiVideo)
        {
            blinkCanvas.SetActive(true);
            
            var materials = aiVideo.MeshRenderer.materials;
            
            var materialIndex = -1;

            for (var i = 0; i < materials.Length; i++)
            {
                if(materials[i].name != aiVideo.originalMaterial.name + " (Instance)")
                    continue;
                materialIndex = i;
                break;
            }
            
            if (materialIndex == -1) yield break;
            
            videoPlayer.clip = aiVideo.videoClip;
            videoPlayer.Prepare();
            
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.isLooping = true;
            
            yield return new WaitForSeconds(MinWaitTime);
            //materials[materialIndex] = videoMaterial;
            materials[materialIndex] = aiVideo.IsPlaneReplace ? transparentVideoMaterial : videoMaterial;
            aiVideo.MeshRenderer.materials = materials;
            
            
            
            if(aiVideo.IsPlaneReplace) aiVideo.MeshRenderer.gameObject.SetActive(true);
            videoPlayer.Play();
            blinkCanvas.SetActive(false);
        }

        public void StopVideoMaterial(string objectName)
        {
            if(!_aiVideoPlayers.TryGetValue(objectName, out var aiVideo)) return;
            if(aiVideo.MeshRenderer == null || aiVideo.videoClip == null || aiVideo.originalMaterial == null) return;
            
            var materials = aiVideo.MeshRenderer.materials;
            
            var materialIndex = -1;

            for (var i = 0; i < materials.Length; i++)
            {
                if(materials[i].name != videoMaterial.name + " (Instance)" && materials[i].name != transparentVideoMaterial.name + " (Instance)")
                    continue;
                materialIndex = i;
                break;
            }
            
            if (materialIndex == -1) return;
            
            materials[materialIndex] = aiVideo.originalMaterial;
            aiVideo.MeshRenderer.materials = materials;
            
            if(aiVideo.IsPlaneReplace) aiVideo.MeshRenderer.gameObject.SetActive(false);
        }
    }

    [Serializable]
    public class AIVideo
    {
        [field: SerializeField] public string AntiqueObjectName { get; set; }
        [field: SerializeField] public MeshRenderer MeshRenderer { get; set; }
        [field: SerializeField] public Material originalMaterial { get; set; }
        [field: SerializeField] public VideoClip videoClip { get; set; }
        [field: SerializeField] public bool IsPlaneReplace { get; set; }
    }
