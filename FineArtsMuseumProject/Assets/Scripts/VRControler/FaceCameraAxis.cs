using UnityEngine;

public class FaceCameraAxis : MonoBehaviour
{
    public Transform cameraTransform;
    public float rotationSmoothSpeed = 5f;

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Chỉ xử lý xoay, KHÔNG chỉnh position!
        Vector3 direction = transform.position - cameraTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}