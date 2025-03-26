using UnityEngine;
using UnityEngine.UI;

public class MagnifierHover : MonoBehaviour
{
    public UnityEngine.Camera mainCamera;
    public UnityEngine.Camera zoomCamera;
    public RawImage magnifierImage; // UI RawImage
    public RectTransform magnifierFrame; // hình viền tròn (tuỳ chọn)
    public LayerMask targetLayer;
    public float zoomFactor = 3f;
    public float cameraOffset = 0.3f;

    void Start()
    {
        magnifierImage.gameObject.SetActive(false);
        if (magnifierFrame != null)
            magnifierFrame.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, targetLayer))
        {
            if (hit.transform.tag == "Player") return;
            
            // Show kính lúp
            magnifierImage.gameObject.SetActive(true);
            if (magnifierFrame != null)
                magnifierFrame.gameObject.SetActive(true);

            magnifierImage.rectTransform.position = mousePos;
            if (magnifierFrame != null)
                magnifierFrame.position = mousePos;

            // Đặt vị trí camera zoom
            Vector3 hitPos = hit.point;
            zoomCamera.transform.position = hitPos + hit.normal * cameraOffset;
            zoomCamera.transform.rotation = Quaternion.LookRotation(-hit.normal);

            // Zoom
            zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;
        }
        else
        {
            magnifierImage.gameObject.SetActive(false);
            if (magnifierFrame != null)
                magnifierFrame.gameObject.SetActive(false);
        }
    }
}