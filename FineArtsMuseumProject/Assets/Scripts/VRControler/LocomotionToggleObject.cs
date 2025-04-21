using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Bật / tắt GameObject khi người chơi đẩy / nhả analog (Continuous Move).
/// Hoạt động cho DynamicMoveProvider, ActionBasedContinuousMoveProvider, v.v.
/// </summary>
public class LocomotionToggleObject : MonoBehaviour
{
    [Header("References")]
    public ContinuousMoveProviderBase moveProvider;   // gán Move (Dynamic Move Provider)
    public GameObject onHandTicket;                   // Bật khi DI CHUYỂN
    public GameObject onHipTicket;                    // Bật khi ĐỨNG YÊN

    [Tooltip("Vector2.sqrMagnitude phải lớn hơn ngưỡng này mới tính là \"đang di chuyển\"")]
    public float moveThreshold = 0.01f;

    void Awake()
    {
        if (moveProvider == null)
            moveProvider = FindObjectOfType<ContinuousMoveProviderBase>();
    }

    void OnEnable()  => Toggle(false);   // mặc định: đứng yên
    void OnDisable() => Toggle(false);

    void Update()
    {
        if (moveProvider == null) return;

        Vector2 axis = ReadMoveAxis(moveProvider);
        bool isMoving = axis.sqrMagnitude > moveThreshold;

        Toggle(isMoving);
    }

    /* ---------- helpers ---------- */

    static Vector2 ReadMoveAxis(ContinuousMoveProviderBase provider)
    {
        // Provider có thể là DynamicMoveProvider hoặc ActionBasedContinuousMoveProvider*
        Vector2 vLeft  = Vector2.zero;
        Vector2 vRight = Vector2.zero;

        // dùng reflection an toàn vì cả hai class đều có 2 trường dưới
        var type = provider.GetType();

        var leftField  = type.GetField("leftHandMoveAction");
        var rightField = type.GetField("rightHandMoveAction");

        if (leftField != null)
        {
            var leftRef = leftField.GetValue(provider) as InputActionReference;
            if (leftRef?.action != null) vLeft = leftRef.action.ReadValue<Vector2>();
        }
        if (rightField != null)
        {
            var rightRef = rightField.GetValue(provider) as InputActionReference;
            if (rightRef?.action != null) vRight = rightRef.action.ReadValue<Vector2>();
        }

        // Trả về vector lớn hơn (nếu chỉ dùng 1 tay vẫn OK)
        return vLeft.sqrMagnitude > vRight.sqrMagnitude ? vLeft : vRight;
    }

    void Toggle(bool moving)
    {
        if (onHandTicket) onHandTicket.SetActive(moving);
        if (onHipTicket)  onHipTicket.SetActive(!moving);
    }
}
