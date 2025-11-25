using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TransformSyncEditor : EditorWindow
{
    private List<GameObject> templateList = new List<GameObject>();
    private List<GameObject> targetList = new List<GameObject>();
    private Vector2 scrollPosition;
    private int successCount = 0;
    private int failCount = 0;

    [MenuItem("Tools/Transform Copier")]
    public static void ShowWindow()
    {
        GetWindow<TransformSyncEditor>("Transform Copier");
    }

    private void OnGUI()
    {
        GUILayout.Label("Transform Copier Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Template list
        GUILayout.Label("Template List (Mẫu - Kéo các GameObject mẫu vào đây):");
        DrawList(ref templateList, "Kéo các GameObject mẫu vào đây");
        GUILayout.Space(10);

        // Target list
        GUILayout.Label("Target List (Đích - Kéo các GameObject cần copy transform vào đây):");
        DrawList(ref targetList, "Kéo các GameObject đích vào đây");
        GUILayout.Space(10);

        // Scale offset info
        EditorGUILayout.HelpBox("Target sẽ có scale + thêm 0.2 so với mẫu", MessageType.Info);
        GUILayout.Space(10);

        // Copy button
        EditorGUI.BeginDisabledGroup(templateList.Count == 0 || targetList.Count == 0);
        if (GUILayout.Button("Copy Transforms", GUILayout.Height(30)))
        {
            CopyTransforms();
        }
        EditorGUI.EndDisabledGroup();

        // Display results
        DisplayResults();
    }

    private void DrawList(ref List<GameObject> list, string placeholder)
    {
        // Hiển thị số lượng hiện tại
        GUILayout.Label($"Số lượng: {list.Count}", EditorStyles.miniLabel);

        // Vùng kéo thả
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, placeholder, EditorStyles.helpBox);
        
        // Hiển thị danh sách hiện tại
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
        
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            list[i] = (GameObject)EditorGUILayout.ObjectField($"#{i} - {GetObjectName(list[i])}", list[i], typeof(GameObject), true);
            
            // Nút xóa từng phần tử
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                list.RemoveAt(i);
                GUIUtility.ExitGUI(); // Thoát GUI để tránh lỗi layout
                return;
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();

        // Nút xóa toàn bộ
        if (list.Count > 0)
        {
            if (GUILayout.Button("Xóa Toàn Bộ"))
            {
                list.Clear();
            }
        }

        // Xử lý kéo thả
        HandleDragAndDrop(dropArea, ref list);
    }

    private string GetObjectName(GameObject obj)
    {
        return obj != null ? obj.name : "Null";
    }

    private void HandleDragAndDrop(Rect dropArea, ref List<GameObject> list)
    {
        Event evt = Event.current;
        
        if (!dropArea.Contains(evt.mousePosition))
            return;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject gameObject)
                        {
                            if (!list.Contains(gameObject))
                            {
                                list.Add(gameObject);
                            }
                        }
                    }
                    
                    evt.Use();
                }
                break;
        }
    }

    private void CopyTransforms()
    {
        successCount = 0;
        failCount = 0;

        if (templateList.Count == 0 || targetList.Count == 0)
        {
            Debug.LogWarning("Cả hai list không được để trống!");
            EditorUtility.DisplayDialog("Lỗi", "Cả hai list không được để trống!", "OK");
            return;
        }

        // Tạo dictionary từ template list để tìm kiếm nhanh hơn
        Dictionary<string, GameObject> templateDict = new Dictionary<string, GameObject>();
        foreach (GameObject template in templateList)
        {
            if (template != null && !templateDict.ContainsKey(template.name + " (1)"))
            {
                templateDict[template.name + " (1)"] = template;
            }
        }

        Undo.RecordObjects(targetList.ToArray(), "Copy Transforms");

        foreach (GameObject targetObj in targetList)
        {
            if (targetObj == null) continue;

            string targetName = targetObj.name;

            if (templateDict.ContainsKey(targetName))
            {
                GameObject templateObj = templateDict[targetName];
                
                // Copy transform values với scale + 0.2
                Undo.RecordObject(targetObj.transform, "Copy Transform");
                targetObj.transform.position = templateObj.transform.position;
                targetObj.transform.rotation = templateObj.transform.rotation;
                
                // Scale + thêm 0.2 so với mẫu
                Vector3 templateScale = templateObj.transform.localScale;
                targetObj.transform.localScale = new Vector3(
                    templateScale.x + 0f,
                    templateScale.y + 0f, 
                    templateScale.z + 0f
                );

                successCount++;
                Debug.Log($"✅ Đã copy transform từ '{templateObj.name}' sang '{targetObj.name}' (Scale: {templateScale} → {targetObj.transform.localScale})");
            }
            else
            {
                failCount++;
                Debug.LogWarning($"❌ Không tìm thấy template cho '{targetName}'");
            }
        }

        // Hiển thị kết quả
        EditorUtility.DisplayDialog("Kết quả", 
            $"Hoàn thành!\n\n✅ Thành công: {successCount}\n❌ Thất bại: {failCount}", "OK");

        // Làm mới scene view để thấy changes
        SceneView.RepaintAll();
    }

    private void DisplayResults()
    {
        GUILayout.Space(10);
        GUILayout.Label("Hướng dẫn:", EditorStyles.boldLabel);
        GUILayout.Label("- Kéo các GameObject mẫu vào Template List");
        GUILayout.Label("- Kéo các GameObject cần copy transform vào Target List");
        GUILayout.Label("- Script sẽ so sánh theo tên và copy transform nếu khớp");
        GUILayout.Label("- Target sẽ có scale + thêm 0.2 so với mẫu");
        GUILayout.Label("- Có thể kéo nhiều GameObject cùng lúc");
        
        if (successCount > 0 || failCount > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("Kết quả lần chạy cuối:", EditorStyles.boldLabel);
            GUILayout.Label($"✅ Thành công: {successCount}");
            GUILayout.Label($"❌ Thất bại: {failCount}");
        }
    }
}