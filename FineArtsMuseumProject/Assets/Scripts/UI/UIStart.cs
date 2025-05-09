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
        
        [field: SerializeField] private Canvas settingCanvas;

        [field: SerializeField] private Animator _animator;
        private Sequence _animationSequence;
        private Sequence _hideSequence;
        private void Awake()
        {
            //_animationSequence = DOTween.Sequence();
            
            //titleImg.color = new Color(titleImg.color.r, titleImg.color.g, titleImg.color.b, 0);
            //subTitleImg.color = new Color(subTitleImg.color.r, subTitleImg.color.g, subTitleImg.color.b, 0);
            //appNameImg.color = new Color(appNameImg.color.r, appNameImg.color.g, appNameImg.color.b, 0);
            //descriptionImg.color = new Color(descriptionImg.color.r, descriptionImg.color.g, descriptionImg.color.b, 0);
            //startBtnImg.color = new Color(startBtnImg.color.r, startBtnImg.color.g, startBtnImg.color.b, 0);
            
            
            //_animationSequence.Append(titleImg.DOFade(1, 0));
            //_animationSequence.Join(subTitleImg.DOFade(1, 0));
            //_animationSequence.Join(appNameImg.DOFade(1, 0));
            //_animationSequence.Join(descriptionImg.DOFade(1, 0));
            //_animationSequence.Join(startBtnImg.DOFade(1, 0));
            //_animationSequence.AppendCallback(() =>
            //{
            //    startBtn.interactable = true;
            //    
            //    startBtn.onClick.AddListener(() =>
            //    {
            //        
            //        startBtn.interactable = false;
            //        UIManager.Instance.DisableUI("UI_START");
            //        if(settingCanvas == null) return;
            //        settingCanvas.gameObject.SetActive(true);
            //        if (PlatformManager.Instance.IsVR)
            //        {
            //            StartRoom.SetActive(false);
            //        }
            //        //UIManager.Instance.EnableUI("UI_GAME");
            //    });
            //});
            
            //_animationSequence.Pause();
            //_animationSequence.SetAutoKill(false);
            
            startBtn.interactable = true;
                
            startBtn.onClick.AddListener(() =>
            {
                if (PlatformManager.Instance.IsVR) return;
                startBtn.interactable = false;
                UIManager.Instance.DisableUI("UI_START");
                if(settingCanvas == null) return;
                settingCanvas.gameObject.SetActive(true);
                
                //UIManager.Instance.EnableUI("UI_GAME");
            });
            
            _hideSequence = DOTween.Sequence();
            
            _hideSequence.Append(titleImg.DOFade(0, 1));
            _hideSequence.Join(subTitleImg.DOFade(0, 1));
            _hideSequence.Join(appNameImg.DOFade(0, 1));
            _hideSequence.Join(descriptionImg.DOFade(0, 1));
            _hideSequence.AppendCallback(() =>
            {
                _animator.SetBool("Clicked", true);
            });
            //_hideSequence.Append(startBtnImg.DOFade(0, 1f));
            // _hideSequence.AppendCallback(() =>
            // {
            //     
            //     startBtn.interactable = false;
            //     
            //     gameObject.SetActive(false);
            //     EnterMain();
            // });
            
            _hideSequence.Pause();
            _hideSequence.SetAutoKill(false);

            if (SceneLog.IsFirstScene) return;
            titleImg.gameObject.SetActive(false);
            subTitleImg.gameObject.SetActive(false);
            appNameImg.gameObject.SetActive(false);
            descriptionImg.gameObject.SetActive(false);
            startBtnImg.gameObject.SetActive(false);
        }
        

        public override void EnableUI()
        {
            if (SceneLog.IsFirstScene)
            {
                gameObject.SetActive(true);
                _animationSequence.Restart();
            }
            else
            {
                EnterMain();
                
            }
        }

        public override void DisableUI()
        {
            _hideSequence.Restart();
        }

        public void EnterMain()
        {
            CharacterManager.Instance.StartControlCharacter();

            if (PlatformManager.Instance.IsWebGL)
            {
                WebRTCManager.Instance.RegisterMoveInput(CharacterManager.Instance.GetActionMove());
            }

            if (PlatformManager.Instance.IsWebGL || PlatformManager.Instance.IsCloud)
            {
                WebRTCManager.Instance.RegisterAudioListener(AudioSubtitleManager.Instance.audioSource);
            }
            
            InputManager.Instance.EnableInput();
            InputManager.Instance.EnableJoystick();
            InputManager.Instance.EnableJoystickRotation();
            UIManager.Instance.EnableUI("UI_NAVIGATION");
            UIManager.Instance.ActionUI("UI_SOUND");
            
            if(SceneLog.IsFirstScene)
                UIManager.Instance.EnableUI("UI_GUIDE");
            
            gameObject.SetActive(false);
        }
    }
}
