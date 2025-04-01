using System;
using DG.Tweening;
using InputController;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIStart : UIBasic
    {
        [field: SerializeField] private TMP_Text titleTxt;
        [field: SerializeField] private TMP_Text subTitleTxt;
        [field: SerializeField] private TMP_Text appNameTxt;
        [field: SerializeField] private TMP_Text descriptionTxt;
        
        [field: SerializeField] private Image startBtnImg;
        [field: SerializeField] private Button startBtn;

        private Sequence _animationSequence;
        private Sequence _hideSequence;
        
        private void Awake()
        {
            _animationSequence = DOTween.Sequence();
            
            titleTxt.alpha = 0;
            subTitleTxt.alpha = 0;
            appNameTxt.alpha = 0;
            descriptionTxt.alpha = 0;
            startBtnImg.color = new Color(1, 1, 1, 0);
            startBtn.interactable = false;
            
            _animationSequence.Append(titleTxt.DOFade(1, 1));
            _animationSequence.Join(subTitleTxt.DOFade(1, 1));
            _animationSequence.Append(appNameTxt.DOFade(1, 1));
            _animationSequence.Join(descriptionTxt.DOFade(1, 1));
            _animationSequence.Append(startBtnImg.DOFade(1, 1));
            _animationSequence.AppendCallback(() =>
            {
                startBtn.interactable = true;
                startBtn.onClick.AddListener(() =>
                {
                    UIManager.Instance.DisableUI("UI_START");
                    //UIManager.Instance.EnableUI("UI_GAME");
                });
            });

            _animationSequence.Pause();
            _animationSequence.SetAutoKill(false);
            
            _hideSequence = DOTween.Sequence();
            
            _hideSequence.Append(titleTxt.DOFade(0, 1));
            _hideSequence.Join(subTitleTxt.DOFade(0, 1));
            _hideSequence.Append(appNameTxt.DOFade(0, 1));
            _hideSequence.Join(descriptionTxt.DOFade(0, 1));
            _hideSequence.Append(startBtnImg.DOFade(0, 1));
            _hideSequence.AppendCallback(() =>
            {
                startBtn.interactable = false;
                gameObject.SetActive(false);
                EnterMain();
            });
            
            _hideSequence.Pause();
            _hideSequence.SetAutoKill(false);
        }

        public override void EnableUI()
        {
            gameObject.SetActive(true);
            _animationSequence.Restart();
        }

        public override void DisableUI()
        {
            _hideSequence.Restart();
        }

        private void EnterMain()
        {
            UIManager.Instance.EnableUI("UI_NAVIGATION");
            UIManager.Instance.EnableUI("UI_GUIDE");
            CharacterManager.Instance.StartControlCharacter();
            InputManager.Instance.EnableInput();
            InputManager.Instance.EnableJoystick();
            InputManager.Instance.EnableJoystickRotation();
        }
    }
}
