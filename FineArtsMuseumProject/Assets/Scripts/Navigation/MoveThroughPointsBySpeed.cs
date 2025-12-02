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

        if (points.Count > 0)
            transform.position = points[0].position;
    }

    private void Update()
    {
        if (points.Count < 2) return;
        if (isWaiting || isStopped) return;

        Transform target = points[currentIndex + 1];
        float step = moveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

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
            isStopped = true;
            yield break;
        }

        isWaiting = false;
    }

    // =============================
    //         PUBLIC METHODS
    // =============================

    public void ResetPath()
    {
        StopAllCoroutines();

        if (points == null || points.Count == 0)
        {
            Debug.Log($"{name}: ResetPath() FAILED — points list is EMPTY!");
            return;
        }

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

    public void StopMoving()
    {
        isStopped = true;

        if (trail != null)
            trail.enabled = false;
    }

    public void StartMoving()
    {
        if (trail != null)
            trail.enabled = true;

        isStopped = false;
    }

    // =============================
    //         GIZMOS
    // =============================

    private void OnDrawGizmos()
    {
        if (points == null || points.Count == 0)
            return;

        Gizmos.color = Color.red;

        // Vẽ điểm
        foreach (var p in points)
        {
            if (p != null)
                Gizmos.DrawSphere(p.position, 0.001f);
        }

        // Vẽ line giữa các điểm
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] != null && points[i + 1] != null)
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
    }
}
