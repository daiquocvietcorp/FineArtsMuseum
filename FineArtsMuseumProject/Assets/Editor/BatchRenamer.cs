using UnityEngine;
using UnityEditor;

public class BatchRenamer : EditorWindow
{
    private string prefix = "NewName_";
    private int startIndex = 0;

    // Tạo menu item để mở cửa sổ batch renamer
    [MenuItem("Tools/Batch Rename")]
    public static void ShowWindow()
    {
        GetWindow<BatchRenamer>("Batch Rename");
    }

    void OnGUI()
    {
        GUILayout.Label("Đổi tên hàng loạt GameObject", EditorStyles.boldLabel);
        prefix = EditorGUILayout.TextField("Tiền tố tên:", prefix);
        startIndex = EditorGUILayout.IntField("Số bắt đầu:", startIndex);

        if (GUILayout.Button("Đổi tên các đối tượng đã chọn"))
        {
            RenameSelectedObjects();
        }
    }

    void RenameSelectedObjects()
    {
        // Lấy danh sách các GameObject đang được chọn trong Hierarchy
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Chưa có đối tượng nào được chọn!");
            return;
        }

        // Thực hiện vòng lặp để đổi tên từng đối tượng
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Undo.RecordObject(selectedObjects[i], "Batch Rename");
            selectedObjects[i].name = prefix + (startIndex + i).ToString();
        }
        Debug.Log("Đổi tên thành công " + selectedObjects.Length + " đối tượng.");
    }
}