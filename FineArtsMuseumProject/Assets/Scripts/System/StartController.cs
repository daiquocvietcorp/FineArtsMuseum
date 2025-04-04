using System.Collections;
using System.Collections.Generic;
using InputController;
using UI;
using UnityEngine;

namespace System
{
    public class StartController : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(RunApplication());
        }

        private IEnumerator RunApplication()
        {
            CanvasManager.Instance.EnableCanvas("MAIN_CANVAS");
            if (PlatformManager.Instance.IsCloud && PlatformManager.Instance.IsTomkoDevice)
            {
                UIManager.Instance.EnableUI("UI_LOADING");
                yield return new WaitForSeconds(13f);
            }
            yield return new WaitForSeconds(.1f);
            
            InputManager.Instance.DisableInput();
            UIManager.Instance.EnableUI("UI_START");
        }
    }
}
