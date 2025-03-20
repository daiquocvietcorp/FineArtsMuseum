using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints; // Danh sách các điểm đến
    [SerializeField] private GameObject arrow; // Object chỉ đường
    [SerializeField] private float moveSpeed = 5f; // Tốc độ di chuyển
    [SerializeField] private float rotationSpeed = 180f; // Tốc độ xoay (độ/giây)
    
    private int currentWaypointIndex = 0;
    private bool isNavigating = true;

    void Start()
    {
        if (waypoints.Count == 0 || arrow == null)
        {
            Debug.LogError("Waypoints list is empty or Arrow object is not assigned.");
            return;
        }

        arrow.transform.position = waypoints[currentWaypointIndex].position;
        StartCoroutine(MoveArrow());
    }

    private IEnumerator MoveArrow()
    {
        while (isNavigating)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];

            while (Vector3.Distance(arrow.transform.position, targetWaypoint.position) > 0.1f)
            {
                // Tính toán hướng cần xoay đến
                Vector3 direction = (targetWaypoint.position - arrow.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Xoay vật thể từ từ về hướng waypoint mới
                arrow.transform.rotation = Quaternion.RotateTowards(
                    arrow.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                // Di chuyển vật thể về waypoint
                arrow.transform.position = Vector3.MoveTowards(
                    arrow.transform.position,
                    targetWaypoint.position,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            // Chuyển sang điểm tiếp theo
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // Quay lại điểm đầu
            }
        }
    }
}