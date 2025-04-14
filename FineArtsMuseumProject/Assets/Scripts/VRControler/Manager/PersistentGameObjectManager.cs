using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentGameObjectManager : MonoBehaviour
{
    // Singleton instance

    [SerializeField] private Button StartButton;
    // Danh sách các GameObject cần giữ lại khi chuyển scene


    private void Start()
    {
        StartButton.onClick.AddListener(StartScene);
    }

    private void StartScene()
    {
        ChangeScene("SceneVR");
        //ChangeScene("Temp");

    }
    public void ChangeScene(string sceneName)
    {
        // Kiểm tra xem scene có tồn tại trong Build Settings hay không
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene \"" + sceneName + "\" không tồn tại trong Build Settings!");
        }
    }
}