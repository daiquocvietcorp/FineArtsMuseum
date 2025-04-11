using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Slider
{
    public class SliderClick : MonoBehaviour, IPointerDownHandler
    {
        private Action _onClickAction;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _onClickAction?.Invoke();
        }
        
        public void RegisterClickAction(Action action)
        {
            _onClickAction = action;
        }
    }
}
