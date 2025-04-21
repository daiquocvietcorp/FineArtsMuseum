using System;
using System.Collections.Generic;
using TMPro;
using Trigger;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Slider
{
    public class SlideHolder : MonoBehaviour
    {
        [field: SerializeField] private Button prevButton;
        [field: SerializeField] private Button nextButton;
        
        [field: SerializeField] private Image sliderImage;
        [field: SerializeField] private TMP_Text sliderTitle;
        [field: SerializeField] private TMP_Text sliderSubtitle;
        
        private List<SliderData> _sliderDataList;
        [SerializeField] private List<TriggerObject> listTriggerObject;
        private int _currentIndex;
        private int _totalSlides;

        private void Start()
        {
            prevButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            sliderImage.gameObject.SetActive(false);
        }

        public void Initialize(List<SliderData> sliderDataList)
        {
            _sliderDataList = sliderDataList;
            _totalSlides = sliderDataList.Count;
            _currentIndex = 0;
            
            if(_totalSlides <= 0) return;
            
            prevButton.onClick.AddListener(OnClickPrevButton);
            nextButton.onClick.AddListener(OnClickNextButton);

            ShowImage(_currentIndex);
        }

        private void ShowImage(int currentIndex)
        {
            if(_sliderDataList == null || _sliderDataList.Count == 0) return;
            if (currentIndex < 0 || currentIndex >= _sliderDataList.Count) return;
            var sliderData = _sliderDataList[currentIndex];
            if (sliderData == null) return;
            sliderImage.sprite = sliderData.sliderSprite;

            if (AudioSubtitleManager.Instance.currentLanguage == "vi")
            {
                sliderTitle.text = sliderData.title;
                sliderSubtitle.text = sliderData.subtitle;
            }
            else
            {
                sliderTitle.text = sliderData.titleEnglish;
                sliderSubtitle.text = sliderData.subtitleEnglish;
            }
        }

        public void OnClickNextButton()
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _totalSlides - 1;
            }
            ShowImage(_currentIndex);
        }

        public void OnClickPrevButton()
        {
            _currentIndex++;
            if (_currentIndex >= _totalSlides)
            {
                _currentIndex = 0;
            }
            ShowImage(_currentIndex);
        }

        public void EnterSlide()
        {
            prevButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            sliderImage.gameObject.SetActive(true);
        }

        public void ExitSlide()
        {
            prevButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            sliderImage.gameObject.SetActive(false);
        }

        public void PresentImage()
        {
            AntiqueManager.Instance.EnableAntiqueDetail(listTriggerObject[_currentIndex].antiqueID);
        }

        public void ChangeLanguage(string currentLanguage)
        {
            if(_sliderDataList == null || _sliderDataList.Count == 0) return;
            if(_sliderDataList[_currentIndex] == null) return;
            
            if (currentLanguage == "vi")
            {
                sliderTitle.text = _sliderDataList[_currentIndex].title;
                sliderSubtitle.text = _sliderDataList[_currentIndex].subtitle;
            }
            else
            {
                sliderTitle.text = _sliderDataList[_currentIndex].titleEnglish;
                sliderSubtitle.text = _sliderDataList[_currentIndex].subtitleEnglish;
            }
        }
    }
}
