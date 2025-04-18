using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class SimpleVRGravity : MonoBehaviour
{
    public float gravity = -9.81f;   // gia tốc rơi
    public float fallSpeed = 0f;     // vận tốc rơi hiện tại
    public float groundOffset = 0.1f; // sai số kiểm tra chạm đất
    public LayerMask groundLayers;   // layer mặt đất

    CharacterController cc;
    XRInteractionManager xrMgr;      // optional – để tắt move provider khi bay

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool isGrounded = CheckGrounded();

        if (isGrounded && fallSpeed < 0)
            fallSpeed = 0f;                     // reset khi chạm đất
        else
            fallSpeed += gravity * Time.deltaTime;  // v = v0 + g·t

        Vector3 move = Vector3.up * fallSpeed * Time.deltaTime;
        cc.Move(move);
    }

    bool CheckGrounded()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.05f;
        return Physics.SphereCast(rayStart, cc.radius, Vector3.down,
                                  out _, cc.height / 2f + groundOffset, groundLayers);
    }
}
