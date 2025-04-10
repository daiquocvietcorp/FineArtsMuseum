using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UI;
using UnityEngine;

public class UIPaintingManager : MonoSingleton<UIPaintingManager>
{
    [field: SerializeField] private List<UIPainting> uiPaintings;
    private Dictionary<string, UIPainting> uiPaintingDict;
    
    private UIPainting _currentUIPainting;
    
    private void Awake()
        {
            uiPaintingDict = new Dictionary<string, UIPainting>();
            _currentUIPainting = null;

            foreach (var pair in uiPaintings)
            {
                //pair.gameObject.SetActive(false);
                uiPaintingDict.Add(pair.GetPaintID(), pair);
            }
        }

        public void EnableUIPainting(string paintID)
        {
            if (!uiPaintingDict.ContainsKey(paintID))
            {
                if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                {
                    paintID += "_pc";
                }

                if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                {
                    paintID += "_mobile";
                }
                
                if (PlatformManager.Instance.IsVR)
                {
                    paintID += "_vr";
                }

                if (PlatformManager.Instance.IsTomko)
                {
                    paintID += "_tomko";
                }

            }
            Debug.Log("paintID:"+ paintID);
            UIManager.Instance.DisableUI("UI_SETTING");
            UIManager.Instance.DisableUI("UI_GUIDE");
            UIManager.Instance.DisableUI("UI_VR");
            //uiPaintingDict[paintID].gameObject.SetActive(true);
            _currentUIPainting = uiPaintingDict[paintID];
        }
        
        public void DisableUIPainting(string paintID)
        {
            if (!_currentUIPainting) return;
            _currentUIPainting.SetDefaultZoom();
            _currentUIPainting = null;
            
            return;
            
            if (uiPaintingDict.ContainsKey(paintID))
            {
                uiPaintingDict[paintID].gameObject.SetActive(false);
                if (_currentUIPainting == uiPaintingDict[paintID])
                {
                    _currentUIPainting = null;
                }
            }
        }

        public void ForceDisableUIPaintingZoom()
        {
            if (_currentUIPainting != null)
            {
                _currentUIPainting.SetDefaultZoom();
                _currentUIPainting.SetDefaultAll();
                _currentUIPainting = null;
            }
        }
}
