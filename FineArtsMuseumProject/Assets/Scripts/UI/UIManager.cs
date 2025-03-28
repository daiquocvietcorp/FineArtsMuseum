using System;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [field: SerializeField] private List<UIObject> uiObjects;
        
        private Dictionary<string, UIBasic> _uiDictionary;
        private bool _isShowingUI;
        private string _currentUIKey;
        
        private void Awake()
        {
            _uiDictionary = new Dictionary<string, UIBasic>();
            foreach (var uiObject in uiObjects)
            {
                _uiDictionary.Add(uiObject.key, uiObject.standaloneUI);
            }
            
            EnableUI("UI_START");
        }
        
        private UIBasic GetUI(string key)
        {
            if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                return uiObjects.Find(x => x.key == key).standaloneUI;
            
            if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                return uiObjects.Find(x => x.key == key).mobileUI;
            
            return null;
        }
        
        public void EnableUI(string key)
        {
            GetUI(key)?.EnableUI();
        }
        
        public void DisableUI(string key)
        {
            GetUI(key)?.DisableUI();
        }

        public void SetDataUI(string key, IUIData data)
        {
            GetUI(key)?.SetData(data);
        }
    }

    public class UIBasic : MonoBehaviour
    {
        public void EnableUI()
        {
            gameObject.SetActive(true);
        }
        
        public void DisableUI()
        {
            gameObject.SetActive(false);
        }
        
        public virtual void SetData(IUIData data)
        {
            
        }
    }

    public interface IUIData
    {
        
    }
}
