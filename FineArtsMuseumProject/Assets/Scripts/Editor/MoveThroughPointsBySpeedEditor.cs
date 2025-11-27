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
    
    private void OnEnable()
    {
        targetScript = (MoveThroughPointsBySpeed)target;
        FindOrCreatePointsContainer();
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
                "Đang trong chế độ tạo điểm. Click vào MeshCollider để tạo điểm mới tại vị trí chính xác.",
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
        isCreatingMode = false;
        isSelectingMode = false;
        isSelectingFloorMode = false;
        
        SceneView.duringSceneGui += OnSceneCreateHeightPairGUI;
        Debug.Log("Chế độ tạo cặp point độ cao đã bật. Click vào vị trí dưới sàn để tạo cặp point.");
    }
    
    private void ToggleSelectingFloorMode()
    {
        isSelectingFloorMode = !isSelectingFloorMode;
        
        if (isSelectingFloorMode)
        {
            isCreatingMode = false;
            isSelectingMode = false;
            SceneView.duringSceneGui += OnSceneSelectingFloorGUI;
            Debug.Log("Chế độ chọn point sàn đã bật. Click vào point có sẵn để làm điểm thấp.");
        }
        else
        {
            SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
            Debug.Log("Chế độ chọn point sàn đã tắt.");
        }
        
        SceneView.RepaintAll();
    }
    
    private void ToggleSelectingMode()
    {
        isSelectingMode = !isSelectingMode;
        
        if (isSelectingMode)
        {
            isCreatingMode = false;
            isSelectingFloorMode = false;
            SceneView.duringSceneGui += OnSceneSelectingGUI;
            Debug.Log("Chế độ chọn đã bật. Click vào các GameObject trong Scene để thêm/xóa khỏi list.");
        }
        else
        {
            SceneView.duringSceneGui -= OnSceneSelectingGUI;
            Debug.Log("Chế độ chọn đã tắt.");
        }
        
        SceneView.RepaintAll();
    }
    
    private void ToggleCreatingMode()
    {
        isCreatingMode = !isCreatingMode;
        
        if (isCreatingMode)
        {
            isSelectingMode = false;
            isSelectingFloorMode = false;
            SceneView.duringSceneGui += OnSceneCreatingGUI;
            Debug.Log("Chế độ tạo điểm đã bật. Click vào MeshCollider để tạo điểm mới.");
        }
        else
        {
            SceneView.duringSceneGui -= OnSceneCreatingGUI;
            Debug.Log("Chế độ tạo điểm đã tắt.");
        }
        
        SceneView.RepaintAll();
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
                
                // Sử dụng point được chọn làm điểm thấp và tạo điểm cao
                CreateHeightPairFromExistingPoint(clickedObject.transform);
                currentEvent.Use();
                
                // Tắt chế độ sau khi tạo
                isSelectingFloorMode = false;
                SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
            }
        }
        
        DrawSceneGUI(sceneView);
        
        // Vẽ hướng dẫn đặc biệt cho chế độ chọn point sàn
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 130, 500, 80));
        GUILayout.Box("Chế Độ Chọn Point Sàn", GUILayout.ExpandWidth(true));
        GUILayout.Label("• Click vào point có sẵn để làm điểm thấp");
        GUILayout.Label("• Point cao sẽ tự động được tạo ở trên trần");
        GUILayout.Label("• Độ cao phụ thuộc vào mesh collider phía trên");
        GUILayout.EndArea();
        Handles.EndGUI();
    }
    
    private void OnSceneCreatingGUI(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        
        Event currentEvent = Event.current;
        
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
                    CreatePointAtPosition(hit.point, meshCollider.gameObject);
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
        
        DrawSceneGUI(sceneView);
        
        // Vẽ hướng dẫn đặc biệt cho chế độ tạo điểm
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 130, 450, 80));
        GUILayout.Box("Chế Độ Tạo Điểm", GUILayout.ExpandWidth(true));
        GUILayout.Label("• Click vào MeshCollider để tạo điểm chính xác");
        GUILayout.Label("• Chỉ hỗ trợ MeshCollider");
        GUILayout.Label("• Trail sẽ tự động về point đầu tiên");
        GUILayout.EndArea();
        Handles.EndGUI();
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
        GUILayout.BeginArea(new Rect(10, 130, 500, 100));
        GUILayout.Box("Chế Độ Tạo Cặp Point Độ Cao", GUILayout.ExpandWidth(true));
        GUILayout.Label("• Click vào vị trí dưới sàn để tạo cặp point");
        GUILayout.Label("• Point thấp sẽ được tạo tại vị trí click");
        GUILayout.Label("• Point cao sẽ tự động được tạo ở trên trần");
        GUILayout.Label("• Độ cao phụ thuộc vào mesh collider phía trên");
        GUILayout.EndArea();
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
        GameObject newPoint = new GameObject($"Point_{targetScript.points.Count + 1}_{suffix}");
        newPoint.tag = "Point";
        newPoint.transform.position = position;
        
        // Nếu có container, đặt point vào container (tùy chọn)
        if (pointsContainer != null)
        {
            newPoint.transform.SetParent(pointsContainer.transform);
        }
        
        // Thêm visual (sphere) để dễ nhìn thấy
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.SetParent(newPoint.transform);
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localScale = Vector3.one * 0.01f;
        
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
        
        // Thêm component để dễ nhận diện
        //PointVisual pointVisual = newPoint.AddComponent<PointVisual>();
        
        // Thêm vào list
        if (targetScript.points == null)
            targetScript.points = new List<Transform>();
            
        targetScript.points.Add(newPoint.transform);
        
        // Undo record
        Undo.RegisterCreatedObjectUndo(newPoint, "Create Height Point");
        
        return newPoint;
    }
    
    private void CreatePointAtPosition(Vector3 position, GameObject hitObject)
    {
        CreatePointAtPositionInternal(position, hitObject, "Normal");
        
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
        GUILayout.BeginArea(new Rect(10, 10, 450, 180));
        GUILayout.Box("Chế Độ Points", GUILayout.ExpandWidth(true));
        GUILayout.Label($"• Mode: {currentMode}");
        GUILayout.Label("• Check Tag: " + (checkTag ? "Bật" : "Tắt"));
        GUILayout.Label("• Đang chọn: " + (Selection.activeGameObject ? Selection.activeGameObject.name : "None"));
        GUILayout.Label("• Trail: " + (trail ? "Đã setup" : "Không có"));
        GUILayout.Label("• Container: " + (pointsContainer ? pointsContainer.name : "Không dùng"));
        GUILayout.Label("• Points trong list: " + targetScript.points.Count);
        GUILayout.Label("• Points đầu tiên: " + (targetScript.points.Count > 0 && targetScript.points[0] != null ? targetScript.points[0].name : "None"));
        GUILayout.EndArea();
        Handles.EndGUI();
        
        DrawExistingPointsVisuals();
        
        // Vẽ vị trí trail nếu có
        //if (trail != null)
        //{
        //    Handles.color = Color.magenta;
        //    Handles.SphereHandleCap(0, targetScript.transform.position, Quaternion.identity, 0.4f, EventType.Repaint);
        //    Handles.Label(targetScript.transform.position + Vector3.up * 0.7f, "Trail Position");
        //}
    }
    
    private void ToggleObjectInList(Transform objectTransform)
    {
        if (targetScript.points == null)
            targetScript.points = new List<Transform>();
        
        if (targetScript.points.Contains(objectTransform))
        {
            targetScript.points.Remove(objectTransform);
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
                    // Xác định màu dựa trên tên point
                    Color pointColor = Color.green;
                    string pointName = targetScript.points[i].name.ToLower();
                    
                    if (pointName.Contains("_low"))
                        pointColor = Color.blue;
                    else if (pointName.Contains("_high"))
                        pointColor = Color.red;
                    
                    // Vẽ point với màu phù hợp
                    Handles.color = pointColor;
                    Handles.SphereHandleCap(0, targetScript.points[i].position, Quaternion.identity, 0.01f, EventType.Repaint);
                    
                    // Vẽ số thứ tự
                    Handles.Label(targetScript.points[i].position + Vector3.up * 0.5f, $"Point {i}");
                    
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
    
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneSelectingGUI;
        SceneView.duringSceneGui -= OnSceneCreatingGUI;
        SceneView.duringSceneGui -= OnSceneCreateHeightPairGUI;
        SceneView.duringSceneGui -= OnSceneSelectingFloorGUI;
    }
}