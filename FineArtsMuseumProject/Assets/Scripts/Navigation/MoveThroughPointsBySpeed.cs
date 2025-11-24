using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MoveThroughPointsBySpeed : MonoBehaviour
{
    [Header("List các điểm")]
    public List<Transform> points = new List<Transform>();

    [Header("Tốc độ di chuyển (units/second)")]
    public float moveSpeed = 3f;

    [Header("Dừng lại tại mỗi điểm")]
    public float stayDuration = 0.1f;

    private int currentIndex = 0;
    private bool isWaiting = false;

    private void Start()
    {
        if (points.Count < 2)
        {
            Debug.LogError("Cần ít nhất 2 điểm để di chuyển.");
            return;
        }

        transform.position = points[0].position;
    }

    private void Update()
    {
        if (isWaiting) return;

        Transform target = points[currentIndex + 1];
        float step = moveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        // Khi chạm đúng point tiếp theo
        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            StartCoroutine(WaitAndNext());
        }
    }

    private IEnumerator WaitAndNext()
    {
        isWaiting = true;

        yield return new WaitForSeconds(stayDuration);

        currentIndex++;

        // Nếu đã đến điểm cuối → dừng luôn
        if (currentIndex >= points.Count - 1)
        {
            yield break;
        }

        isWaiting = false;
    }
}