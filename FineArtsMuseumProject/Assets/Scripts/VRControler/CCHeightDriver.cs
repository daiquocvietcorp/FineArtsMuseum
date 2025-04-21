using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class CCHeightDriver : MonoBehaviour
{
    public XROrigin xrOrigin;
    public float minHeight = 0.8f;
    public float maxHeight = 2.4f;

    CharacterController cc;

    void Awake()
    {
        if (xrOrigin == null) xrOrigin = GetComponent<XROrigin>();
        cc = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        if (xrOrigin == null) return;

        // 1) Tính chiều cao HMD trong local space XR Origin
        float headHeight = Mathf.Clamp(xrOrigin.CameraInOriginSpaceHeight, minHeight, maxHeight);

        // 2) Cập nhật CC.height
        cc.height = headHeight;

        // 3) CC.center = vị trí HMD XZ, + ½ chiều cao
        Vector3 center = xrOrigin.CameraInOriginSpacePos;
        center.y = headHeight / 2f + cc.skinWidth;
        cc.center = center;

        // 4) Dời XR Origin sao cho Capsule đáy chạm đất (nếu cần)
        Vector3 offset = new Vector3(xrOrigin.CameraInOriginSpacePos.x, 0, xrOrigin.CameraInOriginSpacePos.z);
        xrOrigin.transform.Translate(offset);            // move rig
        xrOrigin.CameraFloorOffsetObject.transform.Translate(-offset); // move camera back relative
    }
}
