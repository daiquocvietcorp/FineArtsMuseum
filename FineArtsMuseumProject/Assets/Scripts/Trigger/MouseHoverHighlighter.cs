using UnityEngine;
using System.Collections.Generic;

public class MouseHoverHighlighter : MonoBehaviour
{
    public UnityEngine.Camera mainCamera; // Camera chính
    public LayerMask highlightLayer; // Layer để xác định object có thể được highlight
    public Material highlightMaterial; // Material để làm hiệu ứng khi hover

    private GameObject lastHoveredObject; // Lưu trữ object đang được highlight
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>(); // Lưu Material gốc

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = UnityEngine.Camera.main; // Nếu chưa gán camera, dùng Camera chính
        }
    }

    void Update()
    {
        HandleMouseHover();
    }

    void HandleMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, highlightLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != lastHoveredObject)
            {
                ResetLastHoveredObject(); // Reset object trước đó

                if (hitObject.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
                {
                    lastHoveredObject = hitObject;

                    // Lưu Material gốc nếu chưa có trong Dictionary
                    if (!originalMaterials.ContainsKey(hitObject))
                    {
                        originalMaterials[hitObject] = renderer.materials;
                    }

                    // Tạo danh sách Material mới với Material Highlight
                    Material[] newMaterials = new Material[renderer.materials.Length + 1];
                    renderer.materials.CopyTo(newMaterials, 0);
                    newMaterials[newMaterials.Length - 1] = highlightMaterial;
                    renderer.materials = newMaterials;
                }
            }
        }
        else
        {
            ResetLastHoveredObject();
        }
    }

    void ResetLastHoveredObject()
    {
        if (lastHoveredObject != null && originalMaterials.ContainsKey(lastHoveredObject))
        {
            MeshRenderer renderer = lastHoveredObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.materials = originalMaterials[lastHoveredObject]; // Khôi phục Material gốc
            }

            originalMaterials.Remove(lastHoveredObject);
            lastHoveredObject = null;
        }
    }
}