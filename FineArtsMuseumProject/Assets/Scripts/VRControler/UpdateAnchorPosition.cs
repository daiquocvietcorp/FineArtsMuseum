using UnityEngine;

public class UpdateAnchorPosition : MonoBehaviour
{
    [Tooltip("Offset từ gốc XR Origin đến vị trí hông của người chơi (local space)")]
    public Vector3 hipOffset = new Vector3(0, -0.5f, 0.2f);
    
    private Transform xrOrigin;

    void Start()
    {
        // Giả sử Anchor là con trực tiếp của XR Origin, bạn có thể lấy parent.
        xrOrigin = transform.parent;
        // Nếu không, bạn có thể gán xrOrigin thủ công qua Inspector.
    }

    void LateUpdate()
    {
        // Nếu có xrOrigin, cập nhật local position của Anchor theo hipOffset
        if (xrOrigin != null)
        {
            transform.localPosition = hipOffset;
        }
    }
}