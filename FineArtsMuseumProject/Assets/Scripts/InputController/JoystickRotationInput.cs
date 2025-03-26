using System;
using DesignPatterns;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputController
{
    public class JoystickRotationInput : MonoSingleton<JoystickRotationInput>, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public RectTransform background;
        public RectTransform handle;
        
        private Vector2 _inputVector;
        private Action<Vector2> _onRotate;
        
        private void Awake()
        {
            if (PlatformManager.Instance.IsCloud) return;
            gameObject.SetActive(false);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position = eventData.position - (Vector2)background.position;
            position = Vector2.ClampMagnitude(position, background.sizeDelta.x / 2);

            handle.anchoredPosition = position;
            _inputVector = position / (background.sizeDelta.x / 2);
            
            if(_inputVector == Vector2.zero) return;
            _onRotate?.Invoke(_inputVector);
        }
        
        public void RegisterActionRotate(Action<Vector2> action)
        {
            _onRotate = action;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            handle.anchoredPosition = Vector2.zero;
            _inputVector = Vector2.zero;
            _onRotate?.Invoke(_inputVector);
        }
    }
}
