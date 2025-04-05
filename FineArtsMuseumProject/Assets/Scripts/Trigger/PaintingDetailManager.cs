using System;
using System.Collections.Generic;
using DesignPatterns;
using UI;
using UnityEngine;

namespace Trigger
{
    public class PaintingDetailManager : MonoSingleton<PaintingDetailManager>
    {
        [field: SerializeField] private List<PaintDetail> paintDetails;
        private Dictionary<string, PaintDetail> _paintDetailDict;
        
        private PaintDetail _currentPaintDetail;

        private void Awake()
        {
            _paintDetailDict = new Dictionary<string, PaintDetail>();
            _currentPaintDetail = null;

            foreach (var pair in paintDetails)
            {
                pair.gameObject.SetActive(false);
                _paintDetailDict.Add(pair.GetPaintID(), pair);
            }
        }

        public void EnablePaintDetail(string paintID)
        {
            if (!_paintDetailDict.ContainsKey(paintID))
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
            _paintDetailDict[paintID].gameObject.SetActive(true);
            _currentPaintDetail = _paintDetailDict[paintID];
        }
        
        public void DisablePaintDetail(string paintID)
        {
            if (_paintDetailDict.ContainsKey(paintID))
            {
                _paintDetailDict[paintID].gameObject.SetActive(false);
                if (_currentPaintDetail == _paintDetailDict[paintID])
                {
                    _currentPaintDetail = null;
                }
            }
        }

        public void  ForceDisablePaintDetail()
        {
            if (_currentPaintDetail != null)
            {
                _currentPaintDetail.ClosePanel();
                _currentPaintDetail = null;
                
            }
        }
    }
}
