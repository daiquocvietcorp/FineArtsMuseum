using UnityEngine;

public class FixTrailRotation : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        if (trailRenderer.alignment != LineAlignment.TransformZ)
        {
            trailRenderer.alignment = LineAlignment.TransformZ; // Đảm bảo trail không nhìn theo camera
        }
    }

    void Update()
    {
        // Giữ trail luôn song song mặt đất
        Vector3 forward = transform.forward;
        forward.y = 0; // Giữ hướng di chuyển trên mặt phẳng
        transform.rotation = Quaternion.LookRotation(forward);
    }
}