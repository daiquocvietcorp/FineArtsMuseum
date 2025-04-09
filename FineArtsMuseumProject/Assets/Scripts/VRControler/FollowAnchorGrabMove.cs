using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class FollowAnchorGrabMove : MonoBehaviour
{
    [Header("References")]
    public Transform followAnchor; // Anchor theo nhân vật
    public CharacterControllerDriver controllerDriver;
    public XRGrabInteractable grabInteractable;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 0, 0.2f); // Vị trí Sphere so với anchor
    public float maxGrabDistance = 0.3f;
    public float moveSpeed = 1f;
    public float resetDuration = 0.3f;

    private Vector3 initialLocalPos;     
    private Quaternion initialLocalRot;  
    private bool isGrabbing = false;
    private Vector3 lastWorldPos;
    private CharacterController characterController;

    void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (followAnchor == null)
            Debug.LogWarning("Follow Anchor chưa được gán!");

        initialLocalPos = offset;
        initialLocalRot = Quaternion.identity;

        characterController = controllerDriver.GetComponent<CharacterController>();

        grabInteractable.selectEntered.AddListener(OnGrabStart);
        grabInteractable.selectExited.AddListener(OnGrabEnd);
    }

    void Update()
    {
        if (!isGrabbing && followAnchor != null)
        {
            // Sphere luôn đi theo anchor khi không bị grab
            transform.position = followAnchor.TransformPoint(offset);
            transform.rotation = followAnchor.rotation;
        }

        if (isGrabbing)
        {
            Vector3 worldInitialPos = followAnchor.TransformPoint(initialLocalPos);
            Vector3 currentWorldPos = transform.position;

            Vector3 toInitial = currentWorldPos - worldInitialPos;

            // Giới hạn kéo
            if (toInitial.magnitude > maxGrabDistance)
                toInitial = toInitial.normalized * maxGrabDistance;

            // Khóa trục Y
            toInitial.y = 0f;
            Vector3 clampedWorldPos = worldInitialPos + toInitial;
            transform.position = clampedWorldPos;
            currentWorldPos = clampedWorldPos;

            // Tính movement
            Vector3 movement = lastWorldPos - currentWorldPos;
            movement.y = 0f;

            if (characterController != null)
                characterController.Move(movement * moveSpeed);

            lastWorldPos = currentWorldPos;
        }
    }

    void OnGrabStart(SelectEnterEventArgs args)
    {
        isGrabbing = true;
        lastWorldPos = transform.position;
    }

    void OnGrabEnd(SelectExitEventArgs args)
    {
        isGrabbing = false;
        StopAllCoroutines();
        StartCoroutine(SmoothReset());
    }

    IEnumerator SmoothReset()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 targetPos = followAnchor.TransformPoint(initialLocalPos);
        Quaternion targetRot = followAnchor.rotation;

        float time = 0f;
        while (time < resetDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, time / resetDuration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, time / resetDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}
