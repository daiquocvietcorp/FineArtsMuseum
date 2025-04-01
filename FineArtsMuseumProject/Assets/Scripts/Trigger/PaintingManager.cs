using System;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace Trigger
{
    public class PaintingManager : MonoSingleton<PaintingManager>
    {
        [field: SerializeField] private List<PaintDetail> paintDetails;
        private Dictionary<string, PaintDetail> _paintDetailDict;

        private void Awake()
        {
            _paintDetailDict = new Dictionary<string, PaintDetail>();

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
            }
            Debug.Log("paintID:"+ paintID);
            _paintDetailDict[paintID].gameObject.SetActive(true);
        }
        
        public void DisablePaintDetail(string paintID)
        {
            if (_paintDetailDict.ContainsKey(paintID))
            {
                _paintDetailDict[paintID].gameObject.SetActive(false);
            }
        }
    }
}
