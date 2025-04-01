using System;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace UI
{
    public class CanvasManager : MonoSingleton<CanvasManager>
    {
        [field: SerializeField] private List<CanvasObject> canvasObjects;
        private Dictionary<string, CanvasPlatform> _canvasDictionary;

        public CanvasManager(Dictionary<string, CanvasPlatform> canvasDictionary)
        {
            _canvasDictionary = canvasDictionary;
        }

        public void Awake()
        {
            _canvasDictionary = new Dictionary<string, CanvasPlatform>();
            
            foreach(var canvas in canvasObjects)
            {
                _canvasDictionary.Add(canvas.key, canvas.canvasPlatform);
            }
        }
        
        public void EnableCanvas(string key)
        {
            if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                _canvasDictionary[key].standaloneCanvas.gameObject.SetActive(true);
            
            if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                _canvasDictionary[key].mobileCanvas.gameObject.SetActive(true);
        }
        
        public void DisableCanvas(string key)
        {
            if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                _canvasDictionary[key].standaloneCanvas.gameObject.SetActive(false);
            
            if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                _canvasDictionary[key].mobileCanvas.gameObject.SetActive(false);
        }
    }

    [Serializable]
    public class CanvasPlatform
    {
        public Transform standaloneCanvas;
        public Transform mobileCanvas;
    }
    
    [Serializable]
    public class CanvasObject
    {
        public string key;
        public CanvasPlatform canvasPlatform;
    }
}
