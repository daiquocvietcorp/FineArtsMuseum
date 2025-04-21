using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SphereGrabMover : XRGrabInteractable
{
    [Header("Anchors")]
    public Transform anchor;                       // Anchor gốc (parent khi không grab)

    [Header("Locomotion")]
    public CharacterController characterController;
    public Collider triggerZone;
    public float pullThreshold = 0.1f;
    public float pullDelay    = 0.2f;
    public float moveSpeed    = 1f;
    public float lerpFactor   = 0.1f;

    /* runtime ---------------------------------------------------------- */
    XRRayInteractor stickyInteractor;
    bool manuallyHolding, isPulling;
    float pullTimer;
    Vector3 grabReferencePos;

    /* ------------------------------------------------------------------ */
    void Start()
    {
        /* Gắn Sphere vào anchor ngay từ đầu */
        if (anchor)
        {
            transform.SetParent(anchor, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        if (triggerZone) triggerZone.gameObject.SetActive(false);
    }

    /* ---------------- Grab ------------------------------------------- */
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        /* 1. Tách khỏi anchor để có toàn quyền di chuyển */
        transform.SetParent(null, true);

        /* 2. Ghi interactor & bật TriggerZone */
        if (args.interactorObject is XRRayInteractor ray)
        {
            stickyInteractor = ray;

            if (triggerZone && characterController)
            {
                triggerZone.transform.SetParent(characterController.transform);
                triggerZone.transform.position = ray.attachTransform.position;
                triggerZone.transform.rotation = ray.attachTransform.rotation;
                triggerZone.gameObject.SetActive(true);
            }
        }

        manuallyHolding  = true;
        isPulling        = false;
        pullTimer        = 0f;
        grabReferencePos = transform.position;     // mốc tính kéo
    }

    /* ---------------- Release ---------------------------------------- */
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        manuallyHolding  = false;
        isPulling        = false;
        pullTimer        = 0f;
        stickyInteractor = null;

        if (triggerZone)
        {
            triggerZone.gameObject.SetActive(false);
            triggerZone.transform.SetParent(transform);
        }

        /* 1. Gắn lại anchor & về local (0,0,0) */
        if (anchor)
        {
            transform.SetParent(anchor, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    /* ---------------- Main update ------------------------------------ */
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase phase)
    {
        base.ProcessInteractable(phase);
        if (!isSelected || characterController == null) return;
        if (phase != XRInteractionUpdateOrder.UpdatePhase.Dynamic) return;

        /* A. Lerp Sphere tới tay */
        if (manuallyHolding && stickyInteractor && stickyInteractor.attachTransform)
        {
            var handTf = stickyInteractor.attachTransform;
            transform.position = Vector3.Lerp(transform.position, handTf.position, lerpFactor);

            /* Luôn giữ TriggerZone ở tay */
            if (triggerZone)
            {
                triggerZone.transform.position = handTf.position;
                triggerZone.transform.rotation = handTf.rotation;
            }
        }

        /* B. Tính & kiểm tra kéo */
        Vector3 pullVec = transform.position - grabReferencePos; pullVec.y = 0f;
        float pullMag   = pullVec.magnitude;

        if (pullMag > pullThreshold)
        {
            pullTimer += Time.deltaTime;
            if (pullTimer >= pullDelay) isPulling = true;
        }
        else { pullTimer = 0f; isPulling = false; }

        /* C. Di chuyển XR Origin */
        if (isPulling && triggerZone && pullMag > 0.01f)
        {
            Vector3 dir = transform.position - triggerZone.bounds.center; dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                characterController.Move(dir.normalized * moveSpeed * Time.deltaTime);
        }
    }
}
