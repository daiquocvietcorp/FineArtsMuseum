using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class DualHandRayUpdater : MonoBehaviour
{
    [Header("Attach Transforms")]
    [Tooltip("Attach Transform cho tay trái (theo Index Tip)")]
    public Transform leftRayAttachTransform;

    [Tooltip("Attach Transform cho tay phải (theo Index Tip)")]
    public Transform rightRayAttachTransform;

    private XRHandSubsystem handSubsystem;

    void Start()
    {
        // Lấy XRHandSubsystem từ XR Plugin Management
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader != null)
        {
            handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy XR Loader. Hãy bật XR Plugin Management (OpenXR)!");
        }
    }

    void Update()
    {
        if (handSubsystem == null)
            return;

        // Cập nhật cho tay trái
        UpdateHandRay(handSubsystem.leftHand, leftRayAttachTransform);

        // Cập nhật cho tay phải
        UpdateHandRay(handSubsystem.rightHand, rightRayAttachTransform);
    }

    private void UpdateHandRay(XRHand hand, Transform attachTransform)
    {
        if (attachTransform == null)
            return;

        // Kiểm tra bàn tay có đang tracking không
        if (!hand.isTracked)
            return;

        // Lấy joint IndexTip
        XRHandJoint indexTipJoint = hand.GetJoint(XRHandJointID.IndexTip);

        // Lấy pose của IndexTip
        if (indexTipJoint.TryGetPose(out Pose pose))
        {
            // Gán vị trí + hướng cho Attach Transform
            attachTransform.position = pose.position;
            attachTransform.rotation = pose.rotation;
        }
        else
        {
            // Debug nếu cần
            // Debug.LogWarning("Không lấy được Pose của IndexTip joint!");
        }
    }
}