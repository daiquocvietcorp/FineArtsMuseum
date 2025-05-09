using System;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Slider
{
    public class SlideManager : MonoSingleton<SlideManager>
    {
        [SerializeField] private List<SliderData> topSliderSprites;
        [SerializeField] private List<SliderData> bottomSliderSprites;
        [SerializeField] private SlideHolder topSlideHolder;
        [SerializeField] private SlideHolder bottomSlideHolder;
        
        [SerializeField] private Image topStaticImage;
        [SerializeField] private Image bottomStaticImage;
        [SerializeField] private Button enterButton;
        
        [SerializeField] private SliderClick topSliderClick;
        [SerializeField] private SliderClick bottomSliderClick;
        
        [SerializeField] private SliderDrag topSliderDrag;
        [SerializeField] private SliderDrag bottomSliderDrag;
        
        [SerializeField] private List<BoxCollider> pointerColliders;
        
        private void Start()
        {
            if(!topSlideHolder) return;
            if (!bottomSlideHolder) return;
            topSlideHolder.Initialize(topSliderSprites);
            bottomSlideHolder.Initialize(bottomSliderSprites);
            topStaticImage.gameObject.SetActive(true);
            bottomStaticImage.gameObject.SetActive(true);
            enterButton.gameObject.SetActive(false);
            SetPointerCollider(false);
        }
        
        private void ShowNewsTopPaper()
        {
            topSlideHolder.PresentImage();
            SetPointerCollider(false);
        }
        
        private void ShowNewsBottomPaper()
        {
            bottomSlideHolder.PresentImage();
            SetPointerCollider(false);
        }

        private void ChangeTopPage(int next)
        {
            if(next == -1)
            {
                topSlideHolder.OnClickPrevButton();
                return;
            }
            topSlideHolder.OnClickNextButton();
        }
        
        private void ChangeBottomPage(int next)
        {
            if(next == -1)
            {
                bottomSlideHolder.OnClickPrevButton();
                return;
            }
            bottomSlideHolder.OnClickNextButton();
        }

        private void OnEnterButtonClicked()
        {
            topStaticImage.gameObject.SetActive(false);
            bottomStaticImage.gameObject.SetActive(false);
            enterButton.gameObject.SetActive(false);
            topSlideHolder.EnterSlide();
            bottomSlideHolder.EnterSlide();
            SetPointerCollider(true);
            
            topSliderClick.RegisterClickAction(ShowNewsTopPaper);
            bottomSliderClick.RegisterClickAction(ShowNewsBottomPaper);
            
            topSliderDrag.RegisterDragAction(ChangeTopPage);
            topSliderDrag.RegisterClickAction(SetClickCollider);
            
            bottomSliderDrag.RegisterDragAction(ChangeBottomPage);
            bottomSliderDrag.RegisterClickAction(SetClickCollider);
        }
        
        private void SetClickCollider(bool click)
        {
            topSliderClick.gameObject.SetActive(click);
            bottomSliderClick.gameObject.SetActive(click);
        }

        public void EnterSlideArea()
        {
            Debug.Log("EnterSlideArea");
            if (enterButton)
            {
                enterButton.gameObject.SetActive(true);
                enterButton.onClick.AddListener(OnEnterButtonClicked);
            }
        }

        public void ExitSlideArea()
        {
            if(topSlideHolder)topSlideHolder.ExitSlide();
            if(bottomSlideHolder)bottomSlideHolder.ExitSlide();
            if(topStaticImage)topStaticImage.gameObject.SetActive(true);
            if(bottomStaticImage)bottomStaticImage.gameObject.SetActive(true);
            if(enterButton)enterButton.gameObject.SetActive(false);
            if(enterButton)SetPointerCollider(false);
        }

        public void SetPointerCollider(bool pointer)
        {
            foreach(var point in pointerColliders)
            {
                point.enabled = pointer;
            }
        }

        public void ChangeLanguage(string currentLanguage)
        {
            if(topSlideHolder == null) return;
            if(bottomSlideHolder == null) return;
            topSlideHolder.ChangeLanguage(currentLanguage);
            bottomSlideHolder.ChangeLanguage(currentLanguage);
        }
    }
    
    [Serializable]
    public class SliderData
    {
        [SerializeField] public Sprite sliderSprite;
        [SerializeField] public string title;
        [SerializeField] public string subtitle;
        [SerializeField] public string titleEnglish;
        [SerializeField] public string subtitleEnglish;
    }
}
