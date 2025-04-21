using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Bật / tắt Ticket khi CharacterController thực sự di chuyển (velocity > threshold).
/// </summary>
public class LocomotionToggleObject : MonoBehaviour
{
    [Header("References")]
    [Tooltip("CharacterController gắn cùng XR Origin")]
    public CharacterController xrCharacter;           // Kéo CharacterController (Rig) vào
    [Tooltip("Hiện khi ĐANG di chuyển")]
    public GameObject onHandTicket;                   
    [Tooltip("Hiện khi ĐỨNG yên")]
    public GameObject onHipTicket;

    public GameObject LeftRayCast;

    [Tooltip("Ngưỡng vận tốc (m/s) để xem là đang di chuyển")]
    public float speedThreshold = 0.05f;              // ≈ 5 cm/s

    void Awake()
    {
        if (xrCharacter == null)                       // tự tìm nếu quên gán
            xrCharacter = FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        if (xrCharacter == null) return;

        // CharacterController.velocity là vận tốc hiện tại do Move() sinh ra
        bool moving = xrCharacter.velocity.sqrMagnitude > speedThreshold * speedThreshold;

        if (onHandTicket) onHandTicket.SetActive(moving);
        if (onHipTicket)  onHipTicket.SetActive(!moving);
        if (LeftRayCast) LeftRayCast.SetActive(!moving);
    }
}