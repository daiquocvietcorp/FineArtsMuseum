using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool isStopped = false;

    private TrailRenderer trail;

    private void Start()
    {
        trail = GetComponent<TrailRenderer>();

        if (points.Count < 2)
        {
            Debug.LogError("Cần ít nhất 2 điểm để di chuyển.");
            return;
        }

        transform.position = points[0].position;
    }

    private void Update()
    {
        if (isWaiting || isStopped) return;

        Transform target = points[currentIndex + 1];
        float step = moveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        // Đạt point tiếp theo
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

        if (currentIndex >= points.Count - 1)
        {
            // Đi đến cuối thì dừng
            isStopped = true;
            yield break;
        }

        isWaiting = false;
    }

    // =============================
    //         PUBLIC METHODS
    // =============================

    /// <summary>
    /// Reset về point đầu tiên và chạy lại
    /// </summary>
    public void ResetPath()
    {
        StopAllCoroutines();

        currentIndex = 0;
        isWaiting = false;
        isStopped = false;

        if (trail != null)
        {
            trail.enabled = false;
            transform.position = points[0].position;
            trail.Clear();
            StartCoroutine(EnableTrailNextFrame());
        }
        else
        {
            transform.position = points[0].position;
        }
    }

    private IEnumerator EnableTrailNextFrame()
    {
        yield return null;
        trail.enabled = true;
    }

    /// <summary>
    /// Dừng di chuyển + tắt trail
    /// </summary>
    public void StopMoving()
    {
        isStopped = true;

        if (trail != null)
            trail.enabled = false;
    }

    /// <summary>
    /// Bắt đầu di chuyển trở lại
    /// </summary>
    public void StartMoving()
    {
        if (isStopped)
        {
            isStopped = false;

            if (trail != null)
                trail.enabled = true;
        }
    }
}
