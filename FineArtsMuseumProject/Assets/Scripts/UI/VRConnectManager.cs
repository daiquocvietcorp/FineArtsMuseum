using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class VRConnectManager : UIBasic
    {
        [field: SerializeField] private Button closeBtn;

        public override void SetData(IUIData data)
        {
            base.SetData(data);
            if(data is not UivrData uivrData) return;
            closeBtn.onClick.AddListener(() =>
            {
                uivrData.CloseAction?.Invoke();
                DisableUI();
            });
        }
    }

    public class UivrData : IUIData
    {
        public Action CloseAction;
    }
}
