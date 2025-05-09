using System.Collections;
using System.Collections.Generic;
using InputController;
using UI;
using UnityEngine;

namespace System
{
    public class StartController : MonoBehaviour
    {
        private void Awake()
        {
            if (PlatformManager.Instance.IsVR) return;
            Shader.WarmupAllShaders();
        }

        private void Start()
        {
            if(PlatformManager.Instance.IsVR && SceneLog.IsOpeningScene) return;
            InputManager.Instance.DisableInput();
            if(PlatformManager.Instance.IsTomko) return;
            StartApplication();
        }
        
        public void StartApplication()
        {
            StartCoroutine(RunApplication());
        }

        private IEnumerator RunApplication()
        {
            CanvasManager.Instance.EnableCanvas("MAIN_CANVAS");
            if ((PlatformManager.Instance.IsCloud && PlatformManager.Instance.IsTomkoDevice) || PlatformManager.Instance.IsTomko)
            {
                if (SceneLog.IsFirstScene)
                {
                    UIManager.Instance.EnableUI("UI_LOADING");
                    yield return new WaitForSeconds(7f);
                }
            }
            yield return new WaitForSeconds(.1f);
            
            UIManager.Instance.EnableUI("UI_SOUND");
            UIManager.Instance.EnableUI("UI_START");
        }
    }
}
