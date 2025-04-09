using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SphereGrabMover : XRGrabInteractable
{
    public Transform anchor;                      // Nơi Sphere quay về khi thả
    public CharacterController characterController;
    public float pullThreshold = 0.05f;           // Ngưỡng bắt đầu kéo
    public float moveSpeed = 1f;

    private bool isPulling = false;
    private Vector3 grabReferencePos; // Vị trí snap lúc vừa grab

    void Start()
    {
        if (anchor != null)
            transform.position = anchor.position;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        // Lưu vị trí snap
        grabReferencePos = transform.position;
        isPulling = false;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isPulling = false;
        if (anchor != null)
            transform.position = anchor.position;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (!isSelected || characterController == null)
            return;

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            // Kiểm tra xem đã kéo ra khỏi vị trí ban đầu >= pullThreshold chưa
            Vector3 toSphere = transform.position - grabReferencePos;
            toSphere.y = 0f;
            if (!isPulling && toSphere.magnitude > pullThreshold)
            {
                isPulling = true;
            }

            // Nếu đang kéo -> di chuyển nhân vật ngay theo hướng forward của Sphere
            if (isPulling)
            {
                Vector3 newForward = transform.forward; // Sphere xoay theo tay
                newForward.y = 0f; // Khóa trục Y nếu chỉ muốn di chuyển phẳng
                if (newForward.sqrMagnitude > 0.0001f)
                {
                    newForward.Normalize();
                    characterController.Move(newForward * moveSpeed * Time.deltaTime);
                }
            }
        }
    }
}
