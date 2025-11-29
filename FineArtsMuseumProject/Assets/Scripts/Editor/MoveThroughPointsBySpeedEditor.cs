using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MoveThroughPointsBySpeed))]
public class MoveThroughPointsBySpeedEditor : Editor
{
    private bool isSelectingMode = false;
    private bool isCreatingMode = false;
    private bool isSelectingFloorMode = false;
    private bool checkTag = true; // Toggle kiểm tra tag
    private MoveThroughPointsBySpeed targetScript;
    private GameObject pointsContainer;
    
    // Biến cho preview point
    private GameObject previewPoint;
    private bool showPreview = false;
    
    // Biến mới cho preview line
    private bool showPreviewLine = true;
    
    // Biến cho highlight mesh
    private MeshRenderer hoveredMeshRenderer;
    private Material originalMaterial;
    private Material highlightMaterial;
    private List<Material> originalMaterials = new List<Material>();
    
    private void OnEnable()
    {
        targetScript = (MoveThroughPointsBySpeed)target;
        FindOrCreatePointsContainer();
        CreatePreviewPoint();
        //CreateHighlightMaterial();
    }
    
    private void OnDisable()
    {
        DisableAllModes();
    }
    
    private void DisableAllModes()
    {
        // Tắt tất cả flags
        isSelectingMode = false;
        isCreatingMode = false;
        isSelectingFloorMode = false;
    
        // Hủy đăng ký tất cả sự kiện Scene GUI
        SceneView.duringSceneGui -= OnSceneSelectingGUI;
        SceneView.duringSceneGui -= OnSceneCreatingGUI;
        SceneView.duringSceneGui -= OnSceneCreateHeightPairGUI;
        SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
        SceneView.duringSceneGui -= OnSceneGUI;
    
        // Cleanup
        HidePreviewPoint();
        //RestoreOriginalMaterial();
    
        Debug.Log("Đã tắt tất cả chế độ.");
    }
    
    private void CreateHighlightMaterial()
    {
        if (highlightMaterial == null)
        {
            // Luôn dùng URP Lit shader - chắc chắn có
            highlightMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        
            // Cấu hình đơn giản
            highlightMaterial.color = new Color(1f, 0.8f, 0.2f, 1f); // Màu cam
        
            // Trong URP, đôi khi cần set base color thay vì color
            highlightMaterial.SetColor("_BaseColor", highlightMaterial.color);
        
            // Thêm emission để làm nổi bật
            highlightMaterial.SetColor("_EmissionColor", new Color(0.3f, 0.25f, 0.1f, 1f));
            highlightMaterial.EnableKeyword("_EMISSION");
        
            highlightMaterial.name = "URP_Highlight_Material";
            Debug.Log("✅ Đã tạo URP Lit highlight material");
        }
    }
    
    private void SetHoveredMesh(MeshRenderer newHoveredMesh)
    {
        // Khôi phục mesh cũ nếu có
        RestoreOriginalMaterial();
        
        if (newHoveredMesh != null)
        {
            hoveredMeshRenderer = newHoveredMesh;
            
            // Lưu material gốc
            originalMaterials.Clear();
            foreach (Material mat in hoveredMeshRenderer.sharedMaterials)
            {
                originalMaterials.Add(mat);
            }
            
            // Tạo mảng materials mới với highlight material
            Material[] highlightMaterials = new Material[hoveredMeshRenderer.sharedMaterials.Length];
            for (int i = 0; i < highlightMaterials.Length; i++)
            {
                highlightMaterials[i] = highlightMaterial;
            }
            
            // Áp dụng highlight materials
            hoveredMeshRenderer.sharedMaterials = highlightMaterials;
            
            Debug.Log($"Đã highlight mesh: {hoveredMeshRenderer.gameObject.name}");
        }
    }
    
    private void RestoreOriginalMaterial()
    {
        //if (hoveredMeshRenderer != null && originalMaterials.Count > 0)
        //{
        //    // Khôi phục materials gốc
        //    hoveredMeshRenderer.sharedMaterials = originalMaterials.ToArray();
        //    hoveredMeshRenderer = null;
        //    originalMaterials.Clear();
        //}
    }
    
    
    
    private void DestroyHighlightMaterial()
    {
        if (highlightMaterial != null)
        {
            DestroyImmediate(highlightMaterial);
            highlightMaterial = null;
        }
    }
    
    private void CreatePreviewPoint()
    {
        if (previewPoint == null)
        {
            previewPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            previewPoint.name = "PreviewPoint";
            previewPoint.hideFlags = HideFlags.HideAndDontSave;
            
            // Đặt màu đỏ cho preview
            Renderer renderer = previewPoint.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = new Material(Shader.Find("Standard"));
                renderer.sharedMaterial.color = Color.red;
            }
            
            // Xóa collider
            DestroyImmediate(previewPoint.GetComponent<Collider>());
            
            previewPoint.transform.localScale = Vector3.one * 0.001f;
            previewPoint.SetActive(false);
        }
    }
    
    private void DestroyPreviewPoint()
    {
        if (previewPoint != null)
        {
            DestroyImmediate(previewPoint);
            previewPoint = null;
        }
    }
    
    private void UpdatePreviewPoint(Vector3 position)
    {
        if (previewPoint != null && showPreview)
        {
            previewPoint.transform.position = position;
            previewPoint.SetActive(true);
        }
    }
    
    private void HidePreviewPoint()
    {
        if (previewPoint != null)
        {
            previewPoint.SetActive(false);
        }
    }
    
    public override void OnInspectorGUI()
    {
        // Vẽ inspector mặc định
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        // Toggle kiểm tra tag
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        checkTag = EditorGUILayout.Toggle("Check Tag 'Point'", checkTag);
        EditorGUILayout.HelpBox(
            checkTag ? 
                "Chỉ cho phép chọn các GameObject có tag 'Point'" :
                "Cho phép chọn mọi GameObject",
            MessageType.Info
        );
        
        // Toggle cho preview point
        showPreview = EditorGUILayout.Toggle("Show Preview Point", showPreview);
        EditorGUILayout.HelpBox(
            "Hiển thị điểm preview màu đỏ tại vị trí chuột",
            MessageType.Info
        );
        
        // Toggle mới cho preview line
        showPreviewLine = EditorGUILayout.Toggle("Show Preview Line", showPreviewLine);
        EditorGUILayout.HelpBox(
            "Hiển thị đường kết nối từ preview point đến điểm cuối cùng",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        // Hiển thị thông tin trail
        TrailRenderer trail = targetScript.GetComponent<TrailRenderer>();
        EditorGUILayout.LabelField("Trail Info", EditorStyles.boldLabel);
        
        if (trail != null)
        {
            EditorGUILayout.HelpBox(
                "Trail đã được setup sẵn và sẽ tự động được đặt tại point đầu tiên.",
                MessageType.Info
            );
            
            GUILayout.BeginHorizontal();
            
            // Nút đặt trail về point đầu tiên
            if (GUILayout.Button("Đặt Trail Về Point Đầu Tiên"))
            {
                MoveTrailToFirstPoint();
            }
            
            // Nút clear trail
            if (GUILayout.Button("Clear Trail"))
            {
                trail.Clear();
            }
            
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Không tìm thấy TrailRenderer trên GameObject này.",
                MessageType.Warning
            );
        }
        
        EditorGUILayout.Space();
        
        // Nút tạo cặp point độ cao
        EditorGUILayout.LabelField("Tạo Cặp Point Độ Cao", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Click vào vị trí dưới sàn để tạo điểm thấp, điểm cao sẽ tự động được tạo ở trên trần.",
            MessageType.Info
        );
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Bắt Đầu Tạo Cặp Point Độ Cao"))
        {
            StartCreateHeightPair();
        }
        
        // Nút chọn point sàn
        if (GUILayout.Button(isSelectingFloorMode ? "Dừng Chọn Sàn" : "Chọn Point Sàn"))
        {
            ToggleSelectingFloorMode();
        }
        
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Hiển thị thông tin container (tùy chọn)
        EditorGUILayout.LabelField("Points Container (Optional)", EditorStyles.boldLabel);
        pointsContainer = (GameObject)EditorGUILayout.ObjectField(
            "Container", 
            pointsContainer, 
            typeof(GameObject), 
            true
        );
        
        if (pointsContainer != null)
        {
            EditorGUILayout.HelpBox($"Container đang chứa {pointsContainer.transform.childCount} points", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        GUILayout.BeginHorizontal();
        
        // Nút bật/tắt chế độ chọn
        if (GUILayout.Button(isSelectingMode ? "Dừng Chọn" : "Chọn Points"))
        {
            ToggleSelectingMode();
        }
        
        // Nút bật/tắt chế độ tạo điểm mới
        if (GUILayout.Button(isCreatingMode ? "Dừng Tạo" : "Tạo Points Mới"))
        {
            ToggleCreatingMode();
        }
        
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Nút tạo container mới (tùy chọn)
        if (GUILayout.Button("Tạo Container Mới (Tùy chọn)"))
        {
            CreateNewPointsContainer();
        }
        
        // Hiển thị trạng thái hiện tại
        if (isSelectingMode)
        {
            EditorGUILayout.HelpBox(
                "Đang trong chế độ chọn. Click vào GameObject trong Scene để thêm/xóa khỏi list." +
                (checkTag ? " (Chỉ tag 'Point')" : " (Mọi GameObject)"),
                MessageType.Info
            );
        }
        else if (isCreatingMode)
        {
            EditorGUILayout.HelpBox(
                "Đang trong chế độ tạo điểm. Click vào MeshCollider để tạo điểm mới tại vị trí chính xác." +
                (showPreview ? " (Có preview point)" : " (Không có preview)"),
                MessageType.Info
            );
        }
        else if (isSelectingFloorMode)
        {
            EditorGUILayout.HelpBox(
                "Đang trong chế độ chọn point sàn. Click vào point có sẵn để làm điểm thấp cho cặp độ cao." +
                (checkTag ? " (Chỉ tag 'Point')" : " (Mọi GameObject)"),
                MessageType.Info
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Chọn một chế độ để bắt đầu.",
                MessageType.Info
            );
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Visual Settings", EditorStyles.boldLabel);
        showPreview = EditorGUILayout.Toggle("Show Preview Point", showPreview);
        
        // Có thể thêm toggle cho highlight nếu muốn
        EditorGUILayout.HelpBox(
            "Mesh sẽ được highlight khi hover trong chế độ tạo điểm",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Trạng Thái Hiện Tại", EditorStyles.boldLabel);
    
        string currentMode = "Không có";
        MessageType messageType = MessageType.None;
    
        if (isSelectingMode)
        {
            currentMode = "🎯 CHẾ ĐỘ CHỌN POINT";
            messageType = MessageType.Info;
        }
        else if (isCreatingMode)
        {
            currentMode = "🔄 CHẾ ĐỘ TẠO POINT MỚI";
            messageType = MessageType.Warning;
        }
        else if (isSelectingFloorMode)
        {
            currentMode = "📐 CHẾ ĐỘ CHỌN POINT SÀN";
            messageType = MessageType.Info;
        }
    
        EditorGUILayout.HelpBox(currentMode, messageType);
    
        // Nút tắt tất cả
        if (isSelectingMode || isCreatingMode || isSelectingFloorMode)
        {
            if (GUILayout.Button("🔴 TẮT TẤT CẢ CHẾ ĐỘ"))
            {
                DisableAllModes();
            }
        }
        
        // Nút xóa tất cả points
        if (GUILayout.Button("Xóa Tất Cả Points"))
        {
            if (EditorUtility.DisplayDialog("Xác nhận", "Bạn có chắc muốn xóa tất cả points?", "Xóa", "Hủy"))
            {
                DeleteAllPoints();
            }
        }
    }
    
    private void FindOrCreatePointsContainer()
    {
        // Không tự động tạo container nữa, chỉ tìm nếu có
        string containerName = $"{targetScript.gameObject.name}_PointsContainer";
        pointsContainer = GameObject.Find(containerName);
    }
    
    private void EnsurePointsContainerExists()
    {
        if (pointsContainer == null)
        {
            FindOrCreatePointsContainer();
        
            // Nếu vẫn không tìm thấy, tạo mới
            if (pointsContainer == null)
            {
                CreateNewPointsContainer();
                Debug.Log("🆕 Đã tự động tạo points container mới");
            }
        }
    
        // Kiểm tra container có hợp lệ không
        if (pointsContainer == null)
        {
            Debug.LogError("❌ Không thể tạo hoặc tìm thấy points container!");
        }
    }
    
    private void CreateNewPointsContainer()
    {
        string containerName = $"{targetScript.gameObject.name}_PointsContainer";
        
        // Tạo container mới
        pointsContainer = new GameObject(containerName);
        pointsContainer.transform.position = Vector3.zero;
        
        // Undo record
        Undo.RegisterCreatedObjectUndo(pointsContainer, "Create Points Container");
        
        Debug.Log($"Đã tạo container mới: {containerName}");
        
        Repaint();
    }
    
    private void MoveTrailToFirstPoint()
    {
        TrailRenderer trail = targetScript.GetComponent<TrailRenderer>();
        if (trail != null && targetScript.points != null && targetScript.points.Count > 0)
        {
            if (targetScript.points[0] != null)
            {
                targetScript.transform.position = targetScript.points[0].position;
                trail.Clear();
                Debug.Log($"Đã đặt trail tại point đầu tiên: {targetScript.points[0].position}");
            }
        }
        else
        {
            Debug.LogWarning("Không thể di chuyển trail: thiếu TrailRenderer hoặc points");
        }
    }
    
    private void StartCreateHeightPair()
    {
        // Tắt tất cả chế độ khác
        isCreatingMode = false;
        isSelectingMode = false;
        isSelectingFloorMode = false;
    
        // Hủy đăng ký tất cả sự kiện
        SceneView.duringSceneGui -= OnSceneSelectingGUI;
        SceneView.duringSceneGui -= OnSceneCreatingGUI;
        SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
    
        // Đăng ký sự kiện cho tạo cặp điểm độ cao
        SceneView.duringSceneGui += OnSceneCreateHeightPairGUI;
        HidePreviewPoint();
        RestoreOriginalMaterial();
    
        Debug.Log("Chế độ tạo cặp point độ cao đã bật. Các chế độ khác đã tắt.");
    
        SceneView.RepaintAll();
        Repaint(); // Cập nhật inspector
    }
    
    private void ToggleSelectingFloorMode()
    {
        isSelectingFloorMode = !isSelectingFloorMode;
    
        if (isSelectingFloorMode)
        {
            // Tắt các chế độ khác
            isCreatingMode = false;
            isSelectingMode = false;
        
            // Hủy đăng ký các sự kiện khác
            SceneView.duringSceneGui -= OnSceneSelectingGUI;
            SceneView.duringSceneGui -= OnSceneCreatingGUI;
            SceneView.duringSceneGui -= OnSceneCreateHeightPairGUI;
        
            // Đăng ký sự kiện cho chế độ chọn sàn
            SceneView.duringSceneGui += OnSceneSelectingFloorGUI;
            HidePreviewPoint();
            RestoreOriginalMaterial();
        
            Debug.Log("Chế độ chọn point sàn đã bật. Các chế độ khác đã tắt.");
        }
        else
        {
            SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
            Debug.Log("Chế độ chọn point sàn đã tắt.");
        }
    
        SceneView.RepaintAll();
        Repaint(); // Cập nhật inspector
    }
    
    private void ToggleSelectingMode()
    {
        isSelectingMode = !isSelectingMode;
    
        if (isSelectingMode)
        {
            // Tắt các chế độ khác
            isCreatingMode = false;
            isSelectingFloorMode = false;
        
            // Hủy đăng ký các sự kiện khác
            SceneView.duringSceneGui -= OnSceneCreatingGUI;
            SceneView.duringSceneGui -= OnSceneCreateHeightPairGUI;
            SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
        
            // Đăng ký sự kiện cho chế độ chọn
            SceneView.duringSceneGui += OnSceneSelectingGUI;
            HidePreviewPoint();
            RestoreOriginalMaterial();
        
            Debug.Log("Chế độ chọn đã bật. Các chế độ khác đã tắt.");
        }
        else
        {
            SceneView.duringSceneGui -= OnSceneSelectingGUI;
            Debug.Log("Chế độ chọn đã tắt.");
        }
    
        SceneView.RepaintAll();
        Repaint(); // Cập nhật inspector
    }
    
    private void ToggleCreatingMode()
    {
        isCreatingMode = !isCreatingMode;
    
        if (isCreatingMode)
        {
            // Tắt các chế độ khác
            isSelectingMode = false;
            isSelectingFloorMode = false;
        
            // Hủy đăng ký các sự kiện khác
            SceneView.duringSceneGui -= OnSceneSelectingGUI;
            SceneView.duringSceneGui -= OnSceneCreateHeightPairGUI;
            SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
        
            // Đăng ký sự kiện cho chế độ tạo điểm
            SceneView.duringSceneGui += OnSceneCreatingGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        
            Debug.Log("Chế độ tạo điểm đã bật. Các chế độ khác đã tắt.");
        }
        else
        {
            SceneView.duringSceneGui -= OnSceneCreatingGUI;
            SceneView.duringSceneGui -= OnSceneGUI;
            HidePreviewPoint();
            RestoreOriginalMaterial();
            Debug.Log("Chế độ tạo điểm đã tắt.");
        }
    
        SceneView.RepaintAll();
        Repaint(); // Cập nhật inspector
    }
    
    private void OnSceneSelectingGUI(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        
        Event currentEvent = Event.current;
        
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            GameObject clickedObject = HandleUtility.PickGameObject(currentEvent.mousePosition, false);
            
            if (clickedObject != null)
            {
                // Kiểm tra tag nếu toggle được bật
                if (checkTag && !clickedObject.CompareTag("Point"))
                {
                    Debug.LogWarning($"GameObject {clickedObject.name} không có tag 'Point'.");
                    return;
                }
                
                ToggleObjectInList(clickedObject.transform);
                MoveTrailToFirstPoint();
                currentEvent.Use();
            }
        }
        
        DrawSceneGUI(sceneView);
    }
    
    private void OnSceneSelectingFloorGUI(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    
        Event currentEvent = Event.current;
    
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            GameObject clickedObject = HandleUtility.PickGameObject(currentEvent.mousePosition, false);
        
            if (clickedObject != null)
            {
                // Kiểm tra tag nếu toggle được bật
                if (checkTag && !clickedObject.CompareTag("Point"))
                {
                    Debug.LogWarning($"GameObject {clickedObject.name} không có tag 'Point'.");
                    return;
                }

                if (targetScript.points != null && targetScript.points.Count > 0)
                {
                    targetScript.points.Clear();
                }
            
                // Sử dụng point được chọn làm điểm thấp và tạo điểm cao
                CreateHeightPairFromExistingPoint(clickedObject.transform);
                currentEvent.Use();
            
                // Tắt chế độ sau khi tạo
                isSelectingFloorMode = false;
                SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
            }
        }
    
        DrawSceneGUI(sceneView);
    
        // Vẽ hướng dẫn với GUI
        Handles.BeginGUI();
        Rect areaRect = new Rect(10, 130, 500, 80);
        GUI.Box(areaRect, "Chế Độ Chọn Point Sàn");
    
        Rect labelRect = new Rect(areaRect.x + 10, areaRect.y + 25, areaRect.width - 20, 20);
        GUI.Label(labelRect, "• Click vào point có sẵn để làm điểm thấp");
    
        labelRect.y += 20;
        GUI.Label(labelRect, "• Point cao sẽ tự động được tạo ở trên trần");
    
        labelRect.y += 20;
        GUI.Label(labelRect, "• Độ cao phụ thuộc vào mesh collider phía trên");
    
        Handles.EndGUI();
    }
    
    private void UpdatePreviewAndHighlight(SceneView sceneView)
    {
        // Lấy vị trí chuột hiện tại từ Event.current
        Vector2 mousePosition = Event.current.mousePosition;
    
        // Kiểm tra xem chuột có trong Scene view không
        if (mousePosition.x < 0 || mousePosition.y < 0 || 
            mousePosition.x > sceneView.position.width || 
            mousePosition.y > sceneView.position.height)
        {
            HidePreviewPoint();
            RestoreOriginalMaterial();
            return;
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        // Tìm vị trí trên MeshCollider
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider != null)
            {
                float offset = 0.001f;
                Vector3 previewPosition = hit.point + hit.normal * offset;
            
                // Cập nhật preview point theo toggle showPreview
                if (showPreview)
                {
                    UpdatePreviewPoint(previewPosition); // Hiển thị point
                }
                else
                {
                    HidePreviewPoint(); // Ẩn point
                }
            
                // Highlight mesh được hover
                MeshRenderer meshRenderer = meshCollider.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer != hoveredMeshRenderer)
                {
                    //SetHoveredMesh(meshRenderer);
                }
                return;
            }
        }

        // Nếu không tìm thấy MeshCollider, ẩn preview và bỏ highlight
        HidePreviewPoint();
        RestoreOriginalMaterial();
    }
    
    private void OnSceneCreatingGUI(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    
        Event currentEvent = Event.current;
    
        // LUÔN cập nhật preview (cho cả point và line)
        UpdatePreviewAndHighlight(sceneView);
    
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            Vector2 mousePosition = currentEvent.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
    
            // Chỉ tập trung vào MeshCollider
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider != null)
                {
                    // Truyền cả raycast hit vào để lấy thông tin hướng
                    CreatePointAtPosition(hit.point, hit.normal, meshCollider.gameObject);
                    currentEvent.Use();
    
                    // Tự động đặt trail về point đầu tiên
                    MoveTrailToFirstPoint();
                }
                else
                {
                    Debug.LogWarning("Chỉ hỗ trợ tạo điểm trên MeshCollider.");
                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy MeshCollider tại vị trí click.");
            }
        }
    
        // Vẽ preview info (luôn vẽ nếu có preview point hoặc preview line)
        if (currentEvent.type == EventType.Repaint && (showPreview || showPreviewLine))
        {
            DrawPreviewInfo(sceneView);
        }
    
        DrawSceneGUI(sceneView);
    
        // Vẽ hướng dẫn đặc biệt cho chế độ tạo điểm
        Handles.BeginGUI();
        Rect areaRect = new Rect(10, (showPreview || showPreviewLine) ? 220 : 130, 450, 80);
        GUI.Box(areaRect, "Chế Độ Tạo Điểm");
    
        Rect labelRect = new Rect(areaRect.x + 10, areaRect.y + 25, areaRect.width - 20, 20);
        GUI.Label(labelRect, "• Click vào MeshCollider để tạo điểm chính xác");
    
        labelRect.y += 20;
        GUI.Label(labelRect, "• Chỉ hỗ trợ MeshCollider");
    
        labelRect.y += 20;
        GUI.Label(labelRect, "• Trail sẽ tự động về point đầu tiên");
    
        Handles.EndGUI();
    }
    
    private void UpdatePreviewPointBasedOnMouse(SceneView sceneView)
    {
        // Lấy vị trí chuột hiện tại từ Event.current
        Vector2 mousePosition = Event.current.mousePosition;
    
        // Kiểm tra xem chuột có trong Scene view không
        if (mousePosition.x < 0 || mousePosition.y < 0 || 
            mousePosition.x > sceneView.position.width || 
            mousePosition.y > sceneView.position.height)
        {
            HidePreviewPoint();
            return;
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        // Tìm vị trí trên MeshCollider
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider != null)
            {
                float offset = 0.001f;
                Vector3 previewPosition = hit.point + hit.normal * offset;
                UpdatePreviewPoint(previewPosition);
                return;
            }
        }

        // Nếu không tìm thấy MeshCollider, ẩn preview
        HidePreviewPoint();
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        // Đảm bảo Scene view luôn được repaint để preview mượt mà
        if (isCreatingMode && (showPreview || showPreviewLine))
        {
            sceneView.Repaint();
        }
    }
    
    private void DrawPreviewInfo(SceneView sceneView)
    {
        bool hasValidPreviewPosition = false;
        Vector3 previewPosition = Vector3.zero;
    
        // Lấy vị trí preview từ preview point hoặc raycast
        if (previewPoint != null && previewPoint.activeSelf && showPreview)
        {
            // Sử dụng vị trí từ preview point
            previewPosition = previewPoint.transform.position;
            hasValidPreviewPosition = true;
        }
        else if (showPreviewLine)
        {
            // Nếu chỉ có preview line, lấy vị trí từ raycast
            Vector2 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider != null)
                {
                    float offset = 0.001f;
                    previewPosition = hit.point + hit.normal * offset;
                    hasValidPreviewPosition = true;
                }
            }
        }
    
        if (!hasValidPreviewPosition) return;
    
        // Vẽ thông tin preview với GUI
        Handles.BeginGUI();
        
        string boxTitle = showPreview ? "Preview Info" : "Preview Line Info";
        int extraLines = (showPreviewLine && targetScript.points != null && targetScript.points.Count > 0) ? 2 : 0;
        int hoverExtra = hoveredMeshRenderer != null ? 1 : 0;
        Rect previewRect = new Rect(10, 130, 320, 80 + (extraLines * 20) + (hoverExtra * 20));
        GUI.Box(previewRect, boxTitle);
        
        Rect labelRect = new Rect(previewRect.x + 10, previewRect.y + 25, previewRect.width - 20, 20);
        
        if (showPreview)
        {
            GUI.Label(labelRect, $"Vị trí: {previewPosition:F3}");
            labelRect.y += 20;
        }
        
        if (hoveredMeshRenderer != null)
        {
            GUI.Label(labelRect, $"Mesh: {hoveredMeshRenderer.gameObject.name}");
            labelRect.y += 20;
        }
        
        // Hiển thị thông tin về preview line
        if (showPreviewLine && targetScript.points != null && targetScript.points.Count > 0)
        {
            Transform lastPoint = GetLastValidPoint();
            if (lastPoint != null)
            {
                float distance = Vector3.Distance(previewPosition, lastPoint.position);
                GUI.Label(labelRect, $"Khoảng cách đến cuối: {distance:F2}m");
                labelRect.y += 20;
            }
        }
        
        GUI.Label(labelRect, "Click để tạo điểm tại đây");
        
        Handles.EndGUI();
    
        // Vẽ preview point (chỉ khi được bật)
        if (showPreview && previewPoint != null && previewPoint.activeSelf)
        {
            // Vẽ line từ camera đến preview point
            Handles.color = Color.red;
            Handles.DrawDottedLine(sceneView.camera.transform.position, previewPoint.transform.position, 2f);
            
            // Vẽ sphere tại preview point
            Handles.color = Color.red;
            Handles.SphereHandleCap(0, previewPoint.transform.position, Quaternion.identity, 0.02f, EventType.Repaint);
            
            // Vẽ label tại preview point
            Handles.Label(previewPoint.transform.position + Vector3.up * 0.02f, "Preview Point");
        }
        
        // Vẽ đường đến điểm cuối cùng (chỉ khi preview line được bật)
        if (showPreviewLine)
        {
            DrawPreviewLineToLastPoint(previewPosition);
        }
    }
    
    private Transform GetLastValidPoint()
    {
        if (targetScript.points == null || targetScript.points.Count == 0)
            return null;

        // Tìm điểm cuối cùng hợp lệ trong list
        for (int i = targetScript.points.Count - 1; i >= 0; i--)
        {
            if (targetScript.points[i] != null)
            {
                return targetScript.points[i];
            }
        }
    
        return null;
    }
    
    private void DrawMeshOutline(MeshRenderer meshRenderer)
    {
        // Vẽ bounding box với màu highlight
        Handles.color = new Color(1f, 0.8f, 0.3f, 0.8f); // Màu cam
        Bounds bounds = meshRenderer.bounds;
    
        // Vẽ bounding box chính
        Handles.DrawWireCube(bounds.center, bounds.size);
    
        // Vẽ thêm các đường chéo để dễ nhìn hơn
        Vector3[] corners = GetBoundsCorners(bounds);
        Handles.DrawLine(corners[0], corners[6]); // Đường chéo
        Handles.DrawLine(corners[1], corners[7]); // Đường chéo
        Handles.DrawLine(corners[2], corners[4]); // Đường chéo
        Handles.DrawLine(corners[3], corners[5]); // Đường chéo
    
        // Vẽ sphere tại các góc để dễ nhìn
        Handles.color = new Color(1f, 0.6f, 0.2f, 1f); // Màu cam đậm hơn
        foreach (Vector3 corner in corners)
        {
            Handles.SphereHandleCap(0, corner, Quaternion.identity, 0.05f, EventType.Repaint);
        }
    }
    
    private Vector3[] GetBoundsCorners(Bounds bounds)
    {
        Vector3[] corners = new Vector3[8];
    
        corners[0] = bounds.min;
        corners[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        corners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        corners[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        corners[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        corners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        corners[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        corners[7] = bounds.max;
    
        return corners;
    }
    
    private void OnSceneCreateHeightPairGUI(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        
        Event currentEvent = Event.current;
        
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            Vector2 mousePosition = currentEvent.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            
            // Tìm vị trí click trên sàn
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider != null)
                {
                    CreateHeightPair(hit.point, meshCollider.gameObject);
                    currentEvent.Use();
                    
                    // Tắt chế độ sau khi tạo
                    SceneView.duringSceneGui -= OnSceneCreateHeightPairGUI;
                    
                    // Tự động đặt trail về point đầu tiên
                    MoveTrailToFirstPoint();
                }
                else
                {
                    Debug.LogWarning("Chỉ hỗ trợ tạo điểm trên MeshCollider.");
                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy MeshCollider tại vị trí click.");
            }
        }
        
        DrawSceneGUI(sceneView);
        
        // Vẽ hướng dẫn đặc biệt cho chế độ tạo cặp point độ cao
        Handles.BeginGUI();
        Rect areaRect = new Rect(10, 130, 500, 100);
        GUI.Box(areaRect, "Chế Độ Tạo Cặp Point Độ Cao");
    
        Rect labelRect = new Rect(areaRect.x + 10, areaRect.y + 25, areaRect.width - 20, 20);
        GUI.Label(labelRect, "• Click vào vị trí dưới sàn để tạo cặp point");
    
        labelRect.y += 20;
        GUI.Label(labelRect, "• Point thấp sẽ được tạo tại vị trí click");
    
        labelRect.y += 20;
        GUI.Label(labelRect, "• Point cao sẽ tự động được tạo ở trên trần");
    
        labelRect.y += 20;
        GUI.Label(labelRect, "• Độ cao phụ thuộc vào mesh collider phía trên");
    
        Handles.EndGUI();
    }
    
    private void CreateHeightPairFromExistingPoint(Transform floorPoint)
    {
        // Sử dụng point có sẵn làm điểm thấp
        GameObject lowPoint = floorPoint.gameObject;
        
        // Tìm vị trí trên trần bằng raycast lên trên
        Vector3 ceilingPosition = FindCeilingPosition(floorPoint.position);
        
        // Tạo point cao (trên trần)
        GameObject highPoint = CreatePointAtPositionInternal(ceilingPosition, null, "High");
        targetScript.points.Add(floorPoint);
        MoveTrailToFirstPoint();
        
        Debug.Log($"Đã tạo cặp point độ cao từ point có sẵn:");
        Debug.Log($"- Point thấp: {floorPoint.name} tại {floorPoint.position}");
        Debug.Log($"- Point cao: {ceilingPosition}");
        Debug.Log($"- Khoảng cách: {Vector3.Distance(floorPoint.position, ceilingPosition):F2} units");
    }
    
    private void CreateHeightPair(Vector3 floorPosition, GameObject floorObject)
    {
        // Tạo point thấp (trên sàn)
        GameObject lowPoint = CreatePointAtPositionInternal(floorPosition, floorObject, "Low");
        
        // Tìm vị trí trên trần bằng raycast lên trên
        Vector3 ceilingPosition = FindCeilingPosition(floorPosition);
        
        // Tạo point cao (trên trần)
        GameObject highPoint = CreatePointAtPositionInternal(ceilingPosition, null, "High");
        
        Debug.Log($"Đã tạo cặp point độ cao:");
        Debug.Log($"- Point thấp: {floorPosition}");
        Debug.Log($"- Point cao: {ceilingPosition}");
        Debug.Log($"- Khoảng cách: {Vector3.Distance(floorPosition, ceilingPosition):F2} units");
    }
    
    private Vector3 FindCeilingPosition(Vector3 floorPosition)
    {
        // Raycast lên trên để tìm trần
        Ray ray = new Ray(floorPosition + Vector3.up * 0.1f, Vector3.up);
        RaycastHit hit;
        
        float maxDistance = 100f; // Khoảng cách tối đa để tìm trần
        
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Tìm thấy trần, trả về vị trí va chạm
            return hit.point - Vector3.up * 0.1f; // Điều chỉnh nhỏ để không bị chìm trong mesh
        }
        else
        {
            // Không tìm thấy trần, tạo point cao với độ cao mặc định
            Debug.LogWarning("Không tìm thấy trần, sử dụng độ cao mặc định 10 units");
            return floorPosition + Vector3.up * 10f;
        }
    }
    
    private GameObject CreatePointAtPositionInternal(Vector3 position, GameObject hitObject, string suffix)
    {
        // Tạo GameObject mới
        //GameObject newPoint = new GameObject($"Point_{targetScript.points.Count + 1}_{suffix}");
        //newPoint.tag = "Point";
        //newPoint.transform.position = position;
        
        EnsurePointsContainerExists();
        
        // Thêm visual (sphere) để dễ nhìn thấy
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = $"Point_{targetScript.points.Count + 1}_{suffix}";
        sphere.tag = "Point";
        //sphere.transform.SetParent(newPoint.transform);
        //sphere.transform.localPosition = Vector3.zero;
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.01f;
        sphere.AddComponent<BoxCollider>();
        
        // Nếu có container, đặt point vào container (tùy chọn)
        if (pointsContainer != null)
        {
            sphere.transform.SetParent(pointsContainer.transform);
        }
        
        // Đặt màu cho sphere dựa trên loại point
        Renderer sphereRenderer = sphere.GetComponent<Renderer>();
        if (sphereRenderer != null)
        {
            if (suffix == "Low")
                sphereRenderer.sharedMaterial.color = Color.blue; // Màu xanh cho point thấp
            else if (suffix == "High")
                sphereRenderer.sharedMaterial.color = Color.red;   // Màu đỏ cho point cao
            else
                sphereRenderer.sharedMaterial.color = Color.green; // Màu xanh lá cho point thường
        }
        
        // Xóa collider của sphere để tránh ảnh hưởng đến gameplay
        DestroyImmediate(sphere.GetComponent<Collider>());
        
        // Thêm vào list
        if (targetScript.points == null)
            targetScript.points = new List<Transform>();
            
        targetScript.points.Add(sphere.transform);
        
        // Undo record
        Undo.RegisterCreatedObjectUndo(sphere, "Create Height Point");
        
        return sphere;
    }
    
    private void CreatePointAtPosition(Vector3 position, Vector3 surfaceNormal, GameObject hitObject)
    {
        float offset = 0.0025f; // Offset nhỏ
    
        // Offset theo hướng ngược lại với normal (hướng ra ngoài bề mặt)
        Vector3 finalPosition = position + surfaceNormal * offset;
    
        Debug.Log($"Tạo điểm cách bề mặt {offset} units theo normal: {surfaceNormal}");
    
        CreatePointAtPositionInternal(finalPosition, hitObject, "Normal");
    
        EditorUtility.SetDirty(targetScript);
        if (pointsContainer != null)
        {
            EditorUtility.SetDirty(pointsContainer);
        }
        Repaint();
    }
    
    private void DrawSceneGUI(SceneView sceneView)
    {
        TrailRenderer trail = targetScript.GetComponent<TrailRenderer>();
        
        string currentMode = "None";
        if (isSelectingMode) currentMode = "Chọn Points";
        else if (isCreatingMode) currentMode = "Tạo Điểm Mới";
        else if (isSelectingFloorMode) currentMode = "Chọn Point Sàn";
        
        Handles.BeginGUI();
        
        // Tính chiều cao dynamic dựa trên thông tin
        int extraLines = 0;
        if (showPreview && isCreatingMode) extraLines += 2;
        
        Rect areaRect = new Rect(10, 10, 450, 180 + (extraLines * 20));
        GUI.Box(areaRect, "Chế Độ Points");
        
        Rect labelRect = new Rect(areaRect.x + 10, areaRect.y + 25, areaRect.width - 20, 20);
        GUI.Label(labelRect, $"• Mode: {currentMode}");
        
        labelRect.y += 20;
        GUI.Label(labelRect, $"• Check Tag: {(checkTag ? "Bật" : "Tắt")}");
        
        labelRect.y += 20;
        GUI.Label(labelRect, $"• Preview: {(showPreview ? "Bật" : "Tắt")}");
        
        labelRect.y += 20;
        GUI.Label(labelRect, $"• Đang chọn: {(Selection.activeGameObject ? Selection.activeGameObject.name : "None")}");
        
        labelRect.y += 20;
        GUI.Label(labelRect, $"• Trail: {(trail ? "Đã setup" : "Không có")}");
        
        labelRect.y += 20;
        GUI.Label(labelRect, $"• Container: {(pointsContainer ? pointsContainer.name : "Không dùng")}");
        
        labelRect.y += 20;
        GUI.Label(labelRect, $"• Points trong list: {targetScript.points.Count}");
        
        labelRect.y += 20;
        string firstPointName = targetScript.points.Count > 0 && targetScript.points[0] != null ? targetScript.points[0].name : "None";
        GUI.Label(labelRect, $"• Points đầu tiên: {firstPointName}");
        
        // Hiển thị thông tin điểm cuối cùng nếu đang ở chế độ preview
        if (showPreview && isCreatingMode && targetScript.points.Count > 0)
        {
            Transform lastPoint = GetLastValidPoint();
            if (lastPoint != null)
            {
                labelRect.y += 20;
                GUI.Label(labelRect, $"• Điểm cuối: {lastPoint.name}");
                
                if (previewPoint != null && previewPoint.activeSelf)
                {
                    float distance = Vector3.Distance(previewPoint.transform.position, lastPoint.position);
                    labelRect.y += 20;
                    GUI.Label(labelRect, $"• Khoảng cách preview: {distance:F2}m");
                }
            }
        }
        
        Handles.EndGUI();
        
        DrawExistingPointsVisuals();
    }
    
    private void ToggleObjectInList(Transform objectTransform)
    {
        if (targetScript.points == null)
            targetScript.points = new List<Transform>();
        
        if (targetScript.points.Contains(objectTransform) && targetScript.points[^1] == objectTransform)
        {
            targetScript.points.RemoveAt(targetScript.points.Count - 1);
            Debug.Log($"Đã xóa {objectTransform.name} khỏi list points");
        }
        else
        {
            targetScript.points.Add(objectTransform);
            Debug.Log($"Đã thêm {objectTransform.name} vào list points");
        }
        
        EditorUtility.SetDirty(targetScript);
        Repaint();
    }
    
    private void DrawExistingPointsVisuals()
    {
        if (targetScript.points != null)
        {
            for (int i = 0; i < targetScript.points.Count; i++)
            {
                if (targetScript.points[i] != null)
                {
                    // Xác định màu dựa trên tên point và vị trí
                    Color pointColor = Color.green;
                    string pointName = targetScript.points[i].name.ToLower();
                    
                    if (pointName.Contains("_low"))
                        pointColor = Color.blue;
                    else if (pointName.Contains("_high"))
                        pointColor = Color.red;
                    
                    // Điểm cuối cùng có hiệu ứng đặc biệt khi đang preview
                    bool isLastPoint = (i == targetScript.points.Count - 1);
                    if (isLastPoint && showPreview && isCreatingMode)
                    {
                        pointColor = new Color(0f, 1f, 1f, 1f); // Màu cyan cho điểm cuối khi preview
                    }
                    
                    // Vẽ point với màu phù hợp
                    Handles.color = pointColor;
                    float pointSize = isLastPoint && showPreview && isCreatingMode ? 0.015f : 0.01f;
                    Handles.SphereHandleCap(0, targetScript.points[i].position, Quaternion.identity, pointSize, EventType.Repaint);
                    
                    // Vẽ số thứ tự
                    Handles.Label(targetScript.points[i].position + Vector3.up * 0.05f, $"Point {i}");
                    
                    // Vẽ đường kết nối
                    if (i < targetScript.points.Count - 1 && targetScript.points[i + 1] != null)
                    {
                        Handles.color = Color.yellow;
                        Handles.DrawDottedLine(targetScript.points[i].position, targetScript.points[i + 1].position, 5f);
                    }
                    
                    // Vẽ đường từ point đầu đến point cuối nếu loop
                    if (i == targetScript.points.Count - 1 && targetScript.points.Count > 1 && targetScript.points[0] != null)
                    {
                        Handles.color = Color.cyan;
                        Handles.DrawDottedLine(targetScript.points[i].position, targetScript.points[0].position, 3f);
                    }
                }
            }
        }
    }
    
    private void DeleteAllPoints()
    {
        // Chỉ xóa các points trong list, không xóa container
        if (targetScript.points != null)
        {
            // Xóa từng GameObject point
            foreach (Transform point in targetScript.points)
            {
                if (point != null)
                {
                    DestroyImmediate(point.gameObject);
                }
            }
            
            targetScript.points.Clear();
        }
        
        EditorUtility.SetDirty(targetScript);
        Repaint();
        
        Debug.Log("Đã xóa tất cả points");
    }
    
    private void DrawPreviewLineToLastPoint(Vector3 previewPosition)
    {
        // Chỉ vẽ khi showPreviewLine được bật VÀ có points trong list
        if (!showPreviewLine || targetScript.points == null || targetScript.points.Count == 0)
            return;

        // Tìm điểm cuối cùng hợp lệ trong list
        Transform lastPoint = GetLastValidPoint();
        if (lastPoint == null) return;

        // Vẽ đường từ preview position đến điểm cuối cùng
        Handles.color = new Color(0f, 1f, 1f, 0.8f); // Màu cyan trong suốt
        Handles.DrawDottedLine(previewPosition, lastPoint.position, 3f);
    
        // Vẽ sphere nhỏ tại điểm cuối cùng để dễ nhận biết
        Handles.color = new Color(0f, 1f, 1f, 0.6f);
        Handles.SphereHandleCap(0, lastPoint.position, Quaternion.identity, 0.015f, EventType.Repaint);
    
        // Vẽ label tại điểm cuối cùng
        Handles.Label(lastPoint.position + Vector3.up * 0.03f, "Điểm cuối");
    
        // Vẽ khoảng cách
        float distance = Vector3.Distance(previewPosition, lastPoint.position);
        Vector3 midPoint = (previewPosition + lastPoint.position) / 2f;
        Handles.Label(midPoint + Vector3.up * 0.02f, $"{distance:F2}m");
    }
}