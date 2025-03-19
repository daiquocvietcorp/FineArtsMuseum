using UnityEngine;

public class BasicTestPlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;     // Tốc độ đi bộ
    public float sprintSpeed = 8f;   // Tốc độ chạy nhanh
    public float jumpForce = 7f;     // Lực nhảy
    public float mouseSensitivity = 100f; // Độ nhạy chuột
    public float groundCheckDistance = 0.2f; // Khoảng cách kiểm tra mặt đất
    public LayerMask groundMask; // Layer để kiểm tra mặt đất

    public Transform cameraHolder;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float rotationX = 0f;
    private bool isGrounded;
    private bool isHoldingMouse = false; // Kiểm tra có giữ chuột trái hay không

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Ngăn Rigidbody bị xoay
    }

    void Update()
    {
        HandleMovementInput();
        HandleMouseInput(); // Kiểm tra xem có giữ chuột trái không
        HandleJump();
    }

    void FixedUpdate()
    {
        MoveCharacter();
        CheckGroundStatus(); // Kiểm tra xem nhân vật có đang trên mặt đất không
    }

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed; // Chạy nhanh khi giữ Shift
        moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized * currentSpeed;
    }

    private void MoveCharacter()
    {
        Vector3 moveVelocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        rb.velocity = moveVelocity; // Áp dụng vận tốc mới cho Rigidbody
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false; // Ngăn chặn nhảy liên tục
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Nhấn giữ chuột trái
        {
            isHoldingMouse = true;
        }
        else if (Input.GetMouseButtonUp(0)) // Nhả chuột trái
        {
            isHoldingMouse = false;
        }

        if (isHoldingMouse)
        {
            HandleCameraRotation();
        }
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f; // Xuất phát từ trên chân nhân vật một chút

        if (Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance, groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}