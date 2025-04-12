using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Slider
{
    public class BigSlider : MonoBehaviour
    {
        [field: SerializeField] private SliderDrag sliderDrag;
        [field: SerializeField] private Image sliderImage;
        [field: SerializeField] private List<Sprite> sliderSprites;
        [field: SerializeField] private Sprite defaultSprite;
        
        private int _currentIndex;
        private int _totalSlides;

        private void Start()
        {
            _currentIndex = 0;
            _totalSlides = sliderSprites.Count;
            if (_totalSlides <= 0) return;
            sliderDrag.RegisterDragAction(ChangePage);
            sliderImage.sprite = defaultSprite;
        }

        private void ChangePage(int next)
        {
            if (next == -1)
            {
                _currentIndex--;
                if (_currentIndex < 0)
                {
                    _currentIndex = sliderSprites.Count - 1;
                }
            }
            else
            {
                _currentIndex++;
                if (_currentIndex >= sliderSprites.Count)
                {
                    _currentIndex = 0;
                }
            }

            ShowImage(_currentIndex);
        }

        private void ShowImage(int currentIndex)
        {
            if (sliderSprites == null || sliderSprites.Count == 0) return;
            if (currentIndex < 0 || currentIndex >= sliderSprites.Count) return;
            var sliderData = sliderSprites[currentIndex];
            if (sliderData == null) return;
            sliderImage.sprite = sliderData;
        }
    }
}
