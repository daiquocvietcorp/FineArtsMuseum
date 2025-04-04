using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILoading : UIBasic
    {
        public void OnLoadingScreenCompleted()
        {
            DisableUI();
        }
    }
}
