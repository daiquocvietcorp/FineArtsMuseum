using UnityEngine;

public class TrailOnGround : MonoBehaviour
{
    public LayerMask groundLayer;
    
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out hit, 5f, groundLayer))
        {
            transform.position = hit.point; // Điều chỉnh vị trí xuống mặt đất
        }
    }
}