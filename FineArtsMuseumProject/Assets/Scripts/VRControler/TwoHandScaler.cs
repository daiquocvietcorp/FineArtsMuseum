using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandScaler : MonoBehaviour
{
    [Header("XR Ray Interactors (Auto-Assigned) - Controller")]
    public XRRayInteractor leftControllerRay;
    public XRRayInteractor rightControllerRay;

    [Header("XR Ray Interactors (Auto-Assigned) - Hand Tracking")]
    public XRRayInteractor leftHandRay;
    public XRRayInteractor rightHandRay;

    [Header("Input Actions (Pinch Strength)")]
    [Tooltip("InputActionProperty cho pinch tay trái (controller)")]
    public InputActionProperty leftControllerPinchAction;
    [Tooltip("InputActionProperty cho pinch tay phải (controller)")]
    public InputActionProperty rightControllerPinchAction;
    [Tooltip("InputActionProperty cho pinch tay trái (hand tracking)")]
    public InputActionProperty leftHandPinchAction;
    [Tooltip("InputActionProperty cho pinch tay phải (hand tracking)")]
    public InputActionProperty rightHandPinchAction;

    [Header("Object to Scale")]
    public Transform targetObject;

    [Header("Settings")]
    [Tooltip("Ngưỡng pinch (0-1), trên mức này mới tính là đang pinch")]
    public float pinchThreshold = 0.8f;
    [Tooltip("Tốc độ lerp khi thay đổi scale")]
    public float smoothFactor = 8f;

    // Biến dùng để lưu khoảng cách ban đầu và scale ban đầu
    private float initialDistance = 0f;
    private Vector3 initialScale;

    // ----------------------------------------------------------------
    void Awake()
    {
        // Tự động quét XRRayInteractor trong Scene, chỉ chạy một lần khi scene khởi tạo
        XRRayInteractor[] allInteractors = FindObjectsOfType<XRRayInteractor>();
        foreach (var interactor in allInteractors)
        {
            string lowerName = interactor.gameObject.name.ToLower();
            // Ví dụ "LeftControllerRay"
            if (leftControllerRay == null && lowerName.Contains("left") && lowerName.Contains("controller"))
                leftControllerRay = interactor;
            if (rightControllerRay == null && lowerName.Contains("right") && lowerName.Contains("controller"))
                rightControllerRay = interactor;
            if (leftHandRay == null && lowerName.Contains("left") && lowerName.Contains("hand"))
                leftHandRay = interactor;
            if (rightHandRay == null && lowerName.Contains("right") && lowerName.Contains("hand"))
                rightHandRay = interactor;
        }

        // In ra để debug
        Debug.Log($"TwoHandScaler auto-assigned XRRayInteractors:\n" +
                  $"LeftControllerRay = {leftControllerRay}\nRightControllerRay = {rightControllerRay}\n" +
                  $"LeftHandRay = {leftHandRay}\nRightHandRay = {rightHandRay}");
    }

    // ----------------------------------------------------------------
    void Update()
    {
        // Lấy danh sách các Interactor đang "pinch" trên object này
        var activeInteractors = GetPinchingInteractors();

        if (activeInteractors.Count == 2)
        {
            // Lấy vị trí 2 tay
            Vector3 posA = activeInteractors[0].transform.position;
            Vector3 posB = activeInteractors[1].transform.position;

            float currentDistance = Vector3.Distance(posA, posB);

            // Nếu lần đầu chạm, lưu lại distance và scale
            if (initialDistance == 0f)
            {
                initialDistance = currentDistance;
                initialScale = targetObject.localScale;
            }

            // Tính ratio scale
            float scaleRatio = currentDistance / initialDistance;
            Vector3 targetScale = initialScale * scaleRatio;

            // Lerp mượt
            targetObject.localScale = Vector3.Lerp(targetObject.localScale, targetScale, Time.deltaTime * smoothFactor);
        }
        else
        {
            // Nếu không có hoặc chỉ 1 tay pinch => reset để khi 2 tay pinch lại sẽ tính distance mới
            initialDistance = 0f;
        }
    }

    // ----------------------------------------------------------------
    List<XRRayInteractor> GetPinchingInteractors()
    {
        List<XRRayInteractor> list = new List<XRRayInteractor>();

        // Kiểm tra 4 interactor: controller tay trái, controller tay phải, hand trái, hand phải
        if (leftControllerRay != null) 
            CheckInteractor(leftControllerRay, leftControllerPinchAction.action.ReadValue<float>(), list);
        if (rightControllerRay != null) 
            CheckInteractor(rightControllerRay, rightControllerPinchAction.action.ReadValue<float>(), list);
        if (leftHandRay != null) 
            CheckInteractor(leftHandRay, leftHandPinchAction.action.ReadValue<float>(), list);
        if (rightHandRay != null) 
            CheckInteractor(rightHandRay, rightHandPinchAction.action.ReadValue<float>(), list);

        return list;
    }

    // ----------------------------------------------------------------
    void CheckInteractor(XRRayInteractor interactor, float pinchValue, List<XRRayInteractor> list)
    {
        // Nếu interactor tắt hoặc pinchValue < threshold => bỏ qua
        if (interactor == null || !interactor.enabled) 
            return;
        if (pinchValue < pinchThreshold) 
            return;

        // Raycast trúng object này?
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Chỉ tính pinch nếu object này là object đang scale
            // => so sánh hit.transform == transform hay so sánh targetObject (tùy logic)
            // Ở đây, so sánh "hit.transform == transform" => pinch object đính script
            if (hit.transform == transform)
            {
                list.Add(interactor);
            }
        }
    }
}
