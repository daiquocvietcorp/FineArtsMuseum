using InputController;
using UI;
using UnityEngine;

namespace System
{
    public class StartController : MonoBehaviour
    {
        private void Start()
        {
            InputManager.Instance.DisableInput();
            CanvasManager.Instance.EnableCanvas("MAIN_CANVAS");
            UIManager.Instance.EnableUI("UI_START");
        }
    }
}
