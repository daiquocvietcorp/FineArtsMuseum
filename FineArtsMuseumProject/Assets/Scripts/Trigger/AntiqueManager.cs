using System;
using System.Collections.Generic;
using DesignPatterns;
using UI;
using UnityEngine;

namespace Trigger
{
    public class AntiqueManager : MonoSingleton<AntiqueManager>
    {
        [field: SerializeField] private List<AntiqueObject> antiqueObjects;
        private Dictionary<string, AntiqueObject> _antiqueDetailDict;

        private void Awake()
        {
            _antiqueDetailDict = new Dictionary<string, AntiqueObject>();

            foreach (var pair in antiqueObjects)
            {
                pair.gameObject.SetActive(false);
                _antiqueDetailDict.Add(pair.GetAntiqueID(), pair);
            }
        }

        public void EnableAntiqueDetail(string antiqueID)
        {
            if (!_antiqueDetailDict.ContainsKey(antiqueID))
            {
                if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                {
                    antiqueID += "_pc";
                }

                if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                {
                    antiqueID += "_mobile";
                }
                
                if (PlatformManager.Instance.IsVR)
                {
                    antiqueID += "_vr";
                }
            }
            Debug.Log("AntiqueId:"+ antiqueID);
            _antiqueDetailDict[antiqueID].gameObject.SetActive(true);
            UIManager.Instance.DisableUI("UI_NAVIGATION");
        }
        
        public void DisableAntiqueDetail(string antiqueID)
        {
            if (_antiqueDetailDict.ContainsKey(antiqueID))
            {
                _antiqueDetailDict[antiqueID].gameObject.SetActive(false);
            }
        }

        public void EnableSoundAntique(string antiqueID)
        {
            if (!_antiqueDetailDict.ContainsKey(antiqueID))
            {
                if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                {
                    antiqueID += "_pc";
                }
                if (PlatformManager.Instance.IsVR)
                {
                    antiqueID += "_vr";
                }
                if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                {
                    antiqueID += "_mobile";
                }
            }
            
        }
    }
}
