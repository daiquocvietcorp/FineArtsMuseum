using System;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.UI;

namespace Slider
{
    public class SlideManager : MonoSingleton<SlideManager>
    {
        [SerializeField] private SliderScriptableObject sliderScriptableObject;
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
            topSlideHolder.Initialize(sliderScriptableObject.TopSliderSprites);
            bottomSlideHolder.Initialize(sliderScriptableObject.BottomSliderSprites);
            topStaticImage.gameObject.SetActive(true);
            bottomStaticImage.gameObject.SetActive(true);
            enterButton.gameObject.SetActive(false);
            SetPointerCollider(false);
        }
        
        private void ShowNewsTopPaper()
        {
            Debug.Log("ShowNewsTopPaper");
        }
        
        private void ShowNewsBottomPaper()
        {
            Debug.Log("ShowNewsBottomPaper");
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
            enterButton.gameObject.SetActive(true);
            enterButton.onClick.AddListener(OnEnterButtonClicked);
        }

        public void ExitSlideArea()
        {
            topSlideHolder.ExitSlide();
            bottomSlideHolder.ExitSlide();
            topStaticImage.gameObject.SetActive(true);
            bottomStaticImage.gameObject.SetActive(true);
            enterButton.gameObject.SetActive(false);
            SetPointerCollider(false);
        }

        private void SetPointerCollider(bool pointer)
        {
            foreach(var point in pointerColliders)
            {
                point.enabled = pointer;
            }
        }
    }
}
