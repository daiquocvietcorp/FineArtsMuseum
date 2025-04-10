using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class SphereGrabMover : XRGrabInteractable
{
    [Header("References")]
    [Tooltip("Anchor để Sphere reset khi thả")]
    public Transform anchor;
    
    [Tooltip("CharacterController của XR Origin (dùng để di chuyển nhân vật)")]
    public CharacterController characterController;
    
    [Tooltip("TriggerZone là collider con lớn hơn của Sphere, với IsTrigger = true")]
    public Collider triggerZone;

    [Header("Movement Settings")]
    [Tooltip("Khoảng cách cần kéo để kích hoạt di chuyển nhân vật")]
    public float pullThreshold = 0.1f;
    
    [Tooltip("Thời gian (giây) pull phải vượt qua threshold mới kích hoạt di chuyển")]
    public float pullDelay = 0.2f;
    
    [Tooltip("Tốc độ di chuyển nhân vật")]
    public float moveSpeed = 1f;
    
    [Tooltip("Lerp factor để làm cho Sphere theo sát tay (càng nhỏ càng lag nhiều)")]
    public float lerpFactor = 0.1f;

    // Các biến trạng thái
    private bool isGrabbed = false;
    private bool isPulling = false;
    private float pullTimer = 0f;
    private Vector3 grabReferencePos;  // Vị trí snap ngay sau grab
    private Vector3 movementDirection;
    
    // Dùng cho sticky grab (nếu ray bị ngắt nhưng tay vẫn giữ trigger)
    private XRRayInteractor stickyInteractor;
    private bool manuallyHolding = false;
    
    void Start()
    {
        if (anchor != null)
            transform.position = anchor.position;
        
        if (triggerZone != null)
            triggerZone.gameObject.SetActive(false);
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        
        isGrabbed = true;
        isPulling = false;
        pullTimer = 0f;
        
        // Nếu grab bằng XRRayInteractor (hand tracking ray hoặc controller ray)
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            stickyInteractor = rayInteractor;
            if (rayInteractor.attachTransform != null)
            {
                // Snap Sphere về vị trí tay (Attach Transform)
                transform.position = rayInteractor.attachTransform.position;
                // Lưu lại vị trí snap làm điểm mốc tính pull
                grabReferencePos = transform.position;
                
                // THAY ĐỔI: Gắn triggerZone làm con của characterController để nó cố định theo nhân vật
                if (triggerZone != null && characterController != null)
                {
                    // Đặt triggerZone làm con của nhân vật
                    triggerZone.transform.SetParent(characterController.transform);
                    // Đặt vị trí triggerZone bằng vị trí tay lúc grab (theo attachTransform)
                    triggerZone.transform.position = rayInteractor.attachTransform.position;
                    // Bạn có thể giữ rotation theo tay nếu muốn (hoặc đặt mặc định)
                    triggerZone.transform.rotation = rayInteractor.attachTransform.rotation;
                    triggerZone.gameObject.SetActive(true);
                }
            }
        }
        manuallyHolding = true;
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        isGrabbed = false;
        isPulling = false;
        manuallyHolding = false;
        pullTimer = 0f;
        stickyInteractor = null;
        
        // Reset Sphere về Anchor
        if (anchor != null)
            transform.position = anchor.position;
        
        // Tắt triggerZone và gắn lại triggerZone cho Sphere (nếu cần)
        if (triggerZone != null)
        {
            triggerZone.gameObject.SetActive(false);
            triggerZone.transform.SetParent(transform);
        }
    }
    
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        
        if (!isSelected || characterController == null)
            return;
        
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            // Khi đang grab, cập nhật vị trí của Sphere theo tay bằng Lerp để tạo hiệu ứng lag
            if (manuallyHolding && stickyInteractor != null && stickyInteractor.attachTransform != null)
            {
                Vector3 desiredPos = stickyInteractor.attachTransform.position;
                transform.position = Vector3.Lerp(transform.position, desiredPos, lerpFactor);
            }
            
            // Tính pullVector: hiệu số từ vị trí snap (grabReferencePos) đến vị trí hiện tại của Sphere
            Vector3 pullVector = transform.position - grabReferencePos;
            pullVector.y = 0f;  // Nếu chỉ muốn di chuyển trên mặt phẳng XZ
            
            float pullMagnitude = pullVector.magnitude;
            if (pullMagnitude > pullThreshold)
            {
                pullTimer += Time.deltaTime;
                if (pullTimer >= pullDelay)
                    isPulling = true;
            }
            else
            {
                pullTimer = 0f;
                isPulling = false;
            }
            
            // Nếu đang kéo, di chuyển nhân vật theo hướng từ triggerZone đến Sphere
            if (isPulling && triggerZone != null && pullMagnitude > 0.01f)
            {
                // Lấy tâm của triggerZone (một vị trí cố định theo nhân vật)
                Vector3 triggerCenter = triggerZone.bounds.center;
                // Tính hướng: từ triggerCenter đến vị trí Sphere
                movementDirection = (transform.position - triggerCenter).normalized;
                movementDirection.y = 0f;
                characterController.Move(movementDirection * moveSpeed * Time.deltaTime);
            }
        }
    }
}
