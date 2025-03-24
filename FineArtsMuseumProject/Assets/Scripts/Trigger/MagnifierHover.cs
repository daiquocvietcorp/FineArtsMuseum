using UnityEngine;
using UnityEngine.UI;

public class MagnifierHover : MonoBehaviour
{
    public LayerMask targetLayer; // Layer cần phát hiện
    public UnityEngine.Camera mainCamera;
    public UnityEngine.Camera zoomCamera; // Camera phụ dùng để render vùng zoom
    public RawImage magnifierImage; // UI để hiển thị ảnh zoom
    public float zoomSize = 1f; // Độ lớn của ảnh zoom
    public float zoomFactor = 3f; // Độ phóng to

    void Start()
    {
        if (magnifierImage != null)
        {
            magnifierImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, targetLayer))
        {
            Debug.Log("hit Name:"+hit.transform.name);
            // Hiện hình ảnh phóng to
            magnifierImage.gameObject.SetActive(true);

            // Đặt UI ở vị trí chuột
            magnifierImage.rectTransform.position = mousePos;

            Debug.Log("mousePos "+mousePos);
            
            // Di chuyển camera phụ đến vị trí trúng ray
            Vector3 hitPos = hit.point;
            zoomCamera.transform.position = hitPos + hit.normal * 0.3f; // Cách một chút để nhìn thấy bề mặt
            zoomCamera.transform.rotation = Quaternion.LookRotation(-hit.normal);

            // Điều chỉnh FOV để zoom
            zoomCamera.fieldOfView = mainCamera.fieldOfView / zoomFactor;
        }
        else
        {
            magnifierImage.gameObject.SetActive(false);
        }
    }
}