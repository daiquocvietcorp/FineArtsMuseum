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
                _uiDictionary.Add(uiObject.key, uiObject.ui);
            }
        }
        
        public UIBasic GetUI(string key)
        {
            return _uiDictionary.GetValueOrDefault(key);
        }
        
        public void EnableUI(string key)
        {
            _uiDictionary.GetValueOrDefault(key)?.EnableUI();
        }
        
        public void DisableUI(string key)
        {
            _uiDictionary.GetValueOrDefault(key)?.DisableUI();
        }

        public void SetDataUI(string key, IUIData data)
        {
            _uiDictionary.GetValueOrDefault(key)?.SetData(data);
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
