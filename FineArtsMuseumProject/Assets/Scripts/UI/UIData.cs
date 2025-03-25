using System;
using System.Collections.Generic;
using UnityEngine;

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
        [field: SerializeField] public UIBasic ui;
    }
}
