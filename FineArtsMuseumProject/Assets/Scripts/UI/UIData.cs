using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    [CreateAssetMenu(fileName = "UIData", menuName = "UI/UIData")]
    public class UIData : ScriptableObject
    {
        [field: SerializeField] public List<UIObject> uiObjects;
    }

    [Serializable]
    public class UIObject
    {
        [field: SerializeField] public string key;
        [field: SerializeField] public UIBasic standaloneUI;
        [field: SerializeField] public UIBasic mobileUI;
        [field: SerializeField] public UIBasic vrUI;
        [field: SerializeField] public UIBasic tomkoUI;
        [field: SerializeField] public bool isUsingWeakBlur;
    }
}
