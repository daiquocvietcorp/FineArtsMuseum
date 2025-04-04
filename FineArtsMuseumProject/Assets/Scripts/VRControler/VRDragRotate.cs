using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRDragRotate : XRBaseInteractable
{
    [Tooltip("Cho phép xoay lên/xuống? (nếu false thì chỉ xoay quanh Y)")]
    public bool canRotateUpDown = false;

    [Tooltip("Hệ số nhân tốc độ xoay")]
    public float rotationSpeed = 1f;

    private Transform interactorTransform;  // Tay đang grab
    private Vector3 lastForward;            // Hướng forward cũ của tay
    private bool isDragging = false;        // Đang xoay ?

    // Gọi khi grab bắt đầu
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        interactorTransform = args.interactorObject.transform;
        lastForward = interactorTransform.forward;
        isDragging = true;
    }

    // Gọi khi thả grab
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isDragging = false;
        interactorTransform = null;
    }

    void Update()
    {
        if (isDragging && interactorTransform != null)
        {
            // Lấy hướng forward hiện tại của tay
            Vector3 currentForward = interactorTransform.forward;

            // Tính góc chênh lệch (SignedAngle) giữa hướng cũ và mới
            // - Nếu canRotateUpDown = false, ta xoay quanh Y.
            // - Nếu true, ta xoay quanh trục X (ví dụ).
            Vector3 axis = canRotateUpDown ? Vector3.right : Vector3.up;
            float angleDelta = Vector3.SignedAngle(lastForward, currentForward, axis);

            // Xoay object
            transform.Rotate(axis, angleDelta * rotationSpeed, Space.World);

            // Cập nhật hướng cũ
            lastForward = currentForward;
        }
    }
}