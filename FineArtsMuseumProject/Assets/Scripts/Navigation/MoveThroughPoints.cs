using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MoveThroughPoints : MonoBehaviour
{
    [Header("List các điểm di chuyển theo thứ tự")]
    public List<Transform> points = new List<Transform>();

    [Header("Thời gian di chuyển giữa mỗi điểm")]
    public float moveDuration = 1.5f;

    [Header("Thời gian đứng yên tại mỗi điểm")]
    public float stayDuration = 0.5f;

    private void Start()
    {
        if (points.Count < 2)
        {
            Debug.LogError("Cần ít nhất 2 điểm để di chuyển.");
            return;
        }

        transform.position = points[0].position;  // bắt đầu tại điểm đầu tiên
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 startPos = points[i].position;
            Vector3 endPos = points[i + 1].position;

            float t = 0f;

            // Move from start → end
            while (t < moveDuration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.Clamp01(t / moveDuration);
                transform.position = Vector3.Lerp(startPos, endPos, lerp);
                yield return null;
            }

            // Stay at this point
            yield return new WaitForSeconds(stayDuration);
        }

        // Khi đến điểm cuối thì dừng hẳn
        yield break;
    }
}