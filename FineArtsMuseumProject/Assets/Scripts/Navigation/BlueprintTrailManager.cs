using UnityEngine;

public class BlueprintTrailManager : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float moveDuration = 2f;
    public float stayDuration = 1f;

    private Vector3 startPos;
    private Vector3 endPos;
    private TrailRenderer trail;

    private void Start()
    {
        startPos = startPoint.position;
        endPos = endPoint.position;
        trail = GetComponent<TrailRenderer>();

        transform.position = startPos;
        StartCoroutine(MoveRoutine());
    }

    private System.Collections.IEnumerator MoveRoutine()
    {
        while (true)
        {
            // Move start → end
            float t = 0f;
            while (t < moveDuration)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, t / moveDuration);
                yield return null;
            }
        }
    }
}