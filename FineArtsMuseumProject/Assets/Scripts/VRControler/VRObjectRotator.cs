using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectRotator : MonoBehaviour
{
    [Header("Cài đặt xoay")]
    [Tooltip("Tốc độ xoay (độ/giây)")]
    public float rotationSpeed = 45f;

    [Header("XR Interactors")]
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;
    public bool isPicture;

    private XRRayInteractor activeInteractor = null;
    private bool isRotating = false;

    // Lưu rotation ban đầu của object và rotation ban đầu của tay khi bắt đầu xoay
    private Quaternion initialObjectRotation;
    private Quaternion initialHandRotation;

    void Update()
    {
        // Ưu tiên tay trái
        if (!TryHandleInteractor(leftRayInteractor))
        {
            // Nếu tay trái không hit object, để tay phải xử lý
            TryHandleInteractor(rightRayInteractor);
        }
    }

    bool TryHandleInteractor(XRRayInteractor interactor)
    {
        if (interactor == null || !interactor.enabled) return false;

        // Kiểm tra tia ray trúng object này
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Nếu object gắn script này
            if (hit.transform == transform)
            {
                // Kiểm tra trigger (isSelectActive)
                if (interactor.isSelectActive)
                {
                    // Nếu chưa xoay => lưu rotation
                    if (!isRotating)
                    {
                        StartRotate(interactor);
                    }
                }
                else
                {
                    // Nếu nhả trigger, dừng xoay
                    if (activeInteractor == interactor)
                    {
                        StopRotate();
                    }
                }
                return true;
            }
        }
        else
        {
            // Nếu interactor này đang activeInteractor => dừng xoay
            if (activeInteractor == interactor)
            {
                StopRotate();
            }
        }

        return false;
    }

    void StartRotate(XRRayInteractor interactor)
    {
        isRotating = true;
        activeInteractor = interactor;
        initialObjectRotation = transform.rotation;
        initialHandRotation = interactor.transform.rotation;
    }

    void StopRotate()
    {
        isRotating = false;
        activeInteractor = null;
    }

    void LateUpdate()
    {
        if (isRotating && activeInteractor != null)
        {
            Quaternion currentHandRotation = activeInteractor.transform.rotation;
            Quaternion deltaRot = currentHandRotation * Quaternion.Inverse(initialHandRotation);

            // Xoay object theo deltaRot
            transform.rotation = initialObjectRotation * deltaRot;

            // Nếu muốn chỉ xoay trục Y:
            if (isPicture)
            {
                Vector3 euler = (initialObjectRotation * deltaRot).eulerAngles;
                euler.x = 0;
                euler.z = 0;
                transform.rotation = Quaternion.Euler(euler);
            }
            
        }
    }
}
