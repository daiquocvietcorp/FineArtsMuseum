using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIStart : UIBasic
    {
        [field: SerializeField] private TMP_Text titleTxt;
        [field: SerializeField] private TMP_Text subTitleTxt;
        [field: SerializeField] private TMP_Text appNameTxt;
        [field: SerializeField] private TMP_Text descriptionTxt;

        private Sequence _animationSequence;
        
        private void Awake()
        {
            _animationSequence = DOTween.Sequence();
            
            titleTxt.alpha = 0;
            subTitleTxt.alpha = 0;
            appNameTxt.alpha = 0;
            descriptionTxt.alpha = 0;
            
            _animationSequence.Append(titleTxt.DOFade(1, 1));
            _animationSequence.Join(subTitleTxt.DOFade(1, 1));
            _animationSequence.Append(appNameTxt.DOFade(1, 1));
            _animationSequence.Join(descriptionTxt.DOFade(1, 1));
            _animationSequence.SetAutoKill(false);
        }

        private void Start()
        {
            _animationSequence.Restart();
        }
    }
}
