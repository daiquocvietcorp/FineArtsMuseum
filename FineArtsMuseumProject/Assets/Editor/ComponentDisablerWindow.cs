using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ComponentDisablerWindow : EditorWindow
{
    private List<GameObject> gameObjects = new List<GameObject>();
    private Vector2 scrollPosition;
    
    [MenuItem("Tools/Component Disabler")]
    public static void ShowWindow()
    {
        GetWindow<ComponentDisablerWindow>("Component Disabler");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Component Disabler", EditorStyles.boldLabel);
        
        // Khu vực kéo thả
        GUILayout.Space(10);
        GUILayout.Label("Drag GameObjects from Hierarchy:");
        
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop GameObjects Here");
        
        // Xử lý kéo thả
        HandleDragAndDrop(dropArea);
        
        // Hiển thị danh sách GameObject
        GUILayout.Space(10);
        GUILayout.Label($"Selected Objects ({gameObjects.Count}):");
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        
        for (int i = 0; i < gameObjects.Count; i++)
        {
            GUILayout.BeginHorizontal();
            
            // Hiển thị tên GameObject
            EditorGUILayout.ObjectField(gameObjects[i], typeof(GameObject), true);
            
            // Nút xóa từng item
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                gameObjects.RemoveAt(i);
                i--;
            }
            
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndScrollView();
        
        // Nút xóa tất cả
        if (gameObjects.Count > 0 && GUILayout.Button("Clear All"))
        {
            gameObjects.Clear();
        }
        
        GUILayout.Space(20);
        
        // Nút thực hiện
        EditorGUI.BeginDisabledGroup(gameObjects.Count == 0);
        
        if (GUILayout.Button("Disable Components", GUILayout.Height(30)))
        {
            DisableComponents();
        }
        
        EditorGUI.EndDisabledGroup();
        
        // Hiển thị thông báo
        GUILayout.Space(10);
        GUILayout.Label("Will disable: MeshRenderer and BoxCollider", EditorStyles.helpBox);
    }
    
    private void HandleDragAndDrop(Rect dropArea)
    {
        Event currentEvent = Event.current;
        
        switch (currentEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(currentEvent.mousePosition))
                    return;
                
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        if (obj is GameObject gameObject)
                        {
                            if (!gameObjects.Contains(gameObject))
                            {
                                gameObjects.Add(gameObject);
                            }
                        }
                    }
                    
                    currentEvent.Use();
                }
                break;
        }
    }
    
    private void DisableComponents()
    {
        if (gameObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("No Objects", "Please drag some GameObjects first.", "OK");
            return;
        }
        
        int meshRenderersDisabled = 0;
        int boxCollidersDisabled = 0;
        
        // Bắt đầu undo group
        Undo.RecordObjects(gameObjects.ToArray(), "Disable Components");
        
        foreach (GameObject go in gameObjects)
        {
            if (go == null) continue;
            
            // Tắt MeshRenderer nếu có
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.enabled)
            {
                meshRenderer.enabled = false;
                meshRenderersDisabled++;
            }
            
            // Tắt BoxCollider nếu có
            BoxCollider boxCollider = go.GetComponent<BoxCollider>();
            if (boxCollider != null && boxCollider.enabled)
            {
                boxCollider.enabled = false;
                boxCollidersDisabled++;
            }
            
            // Cũng có thể thêm cho Collider (nếu không chỉ muốn BoxCollider)
            // Collider collider = go.GetComponent<Collider>();
            // if (collider != null && collider.enabled)
            // {
            //     collider.enabled = false;
            // }
        }
        
        // Hiển thị kết quả
        EditorUtility.DisplayDialog("Components Disabled", 
            $"Disabled:\n" +
            $"{meshRenderersDisabled} MeshRenderer(s)\n" +
            $"{boxCollidersDisabled} BoxCollider(s)", 
            "OK");
        
        // Làm mới scene view để thấy thay đổi
        EditorApplication.RepaintHierarchyWindow();
    }
}