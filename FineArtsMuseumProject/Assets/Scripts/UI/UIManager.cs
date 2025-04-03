using System;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [field: SerializeField] private List<UIObject> uiObjects;
        [field: SerializeField] private Transform weakBlur;
        
        private Dictionary<string, UIBasic> _uiDictionary;
        private Dictionary<string, bool> _usingWeakBlurDict;
        
        private bool _isShowingUI;
        private string _currentUIKey;
        
        public UIManager(Dictionary<string, UIBasic> uiDictionary)
        {
            _uiDictionary = uiDictionary;
        }
        
        private void Awake()
        {
            _uiDictionary = new Dictionary<string, UIBasic>();
            _usingWeakBlurDict = new Dictionary<string, bool>();
            foreach (var uiObject in uiObjects)
            {
                if(uiObject.standaloneUI != null)
                    _uiDictionary.Add(uiObject.key + "_PC", uiObject.standaloneUI);
                if(uiObject.mobileUI != null)
                    _uiDictionary.Add(uiObject.key + "_MOBILE", uiObject.mobileUI);
                _usingWeakBlurDict.Add(uiObject.key, uiObject.isUsingWeakBlur);
                if(uiObject.vrUI != null)
                    _uiDictionary.Add(uiObject.key + "_VR", uiObject.vrUI);
            }
        }
        
        private UIBasic GetUI(string key)
        {
            if(PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
                key += "_PC";
            
            if(PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsCloud)
                key += "_MOBILE";
            
            if(PlatformManager.Instance.IsVR)
                key += "_VR";

            return _uiDictionary.GetValueOrDefault(key, null);
        }

        private bool IsUsingWeakBlur(string key)
        {
            return _usingWeakBlurDict.GetValueOrDefault(key, false);
        }
        
        public void EnableUI(string key)
        {
            var ui = GetUI(key);
            if(ui == null) return;
            ui.EnableUI();
            
            if(!IsUsingWeakBlur(key)) return;
            weakBlur.gameObject.SetActive(true);
        }
        
        public void DisableUI(string key)
        {
            var ui = GetUI(key);
            if (ui == null) return;
            ui.DisableUI();
            
            if(!IsUsingWeakBlur(key)) return;
            weakBlur.gameObject.SetActive(false);
        }

        public void SetDataUI(string key, IUIData data)
        {
            GetUI(key)?.SetData(data);
        }
        
        public void ActionUI(string key, Action action = null)
        {
            GetUI(key)?.ActionUI(action);
        }
    }

    public class UIBasic : MonoBehaviour
    {
        public virtual void EnableUI()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void DisableUI()
        {
            gameObject.SetActive(false);
        }
        
        public virtual void SetData(IUIData data)
        {
            
        }
        
        public virtual void ActionUI(Action action = null)
        {
            action?.Invoke();
        }
    }

    public interface IUIData
    {
        
    }
}
