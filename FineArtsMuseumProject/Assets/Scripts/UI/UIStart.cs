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
        [field: SerializeField] private Image titleImg;
        [field: SerializeField] private Image subTitleImg;
        [field: SerializeField] private Image appNameImg;
        [field: SerializeField] private Image descriptionImg;
        
        [field: SerializeField] private Image startBtnImg;
        [field: SerializeField] private Button startBtn;

        private Sequence _animationSequence;
        private Sequence _hideSequence;
        
        private void Awake()
        {
            _animationSequence = DOTween.Sequence();

            
                titleImg.color = new Color(titleImg.color.r, titleImg.color.g, titleImg.color.b, 0);
                subTitleImg.color = new Color(subTitleImg.color.r, subTitleImg.color.g, subTitleImg.color.b, 0);
                appNameImg.color = new Color(appNameImg.color.r, appNameImg.color.g, appNameImg.color.b, 0);
                descriptionImg.color = new Color(descriptionImg.color.r, descriptionImg.color.g, descriptionImg.color.b, 0);
                startBtnImg.color = new Color(startBtnImg.color.r, startBtnImg.color.g, startBtnImg.color.b, 0);
            
            
            _animationSequence.Append(titleImg.DOFade(1, 1));
            _animationSequence.Join(subTitleImg.DOFade(1, 1));
            _animationSequence.Append(appNameImg.DOFade(1, 1));
            _animationSequence.Join(descriptionImg.DOFade(1, 1));
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
            
            _hideSequence.Append(titleImg.DOFade(0, 1));
            _hideSequence.Join(subTitleImg.DOFade(0, 1));
            _hideSequence.Append(appNameImg.DOFade(0, 1));
            _hideSequence.Join(descriptionImg.DOFade(0, 1));
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
