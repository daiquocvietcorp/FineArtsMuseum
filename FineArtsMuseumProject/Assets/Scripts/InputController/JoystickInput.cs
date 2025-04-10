using System;
using DesignPatterns;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputController
{
    public class JoystickInput : MonoSingleton<JoystickInput>, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public RectTransform background;
        public RectTransform handle;

        private Vector2 _inputVector;
        private Action<Vector2> _onMove;
        
        private int _joystickFingerId = -1;

        public Vector2 InputVector => _inputVector;
        
        private void Awake()
        {
            if(PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsTomko)
            {
                CharacterManager.Instance.RegisterActionDefault();
                return;
            }
            gameObject.SetActive(false);
        }


        public void EnableJoystick()
        {
            if(!PlatformManager.Instance.IsMobile && !PlatformManager.Instance.IsCloud) return;
            gameObject.SetActive(true);
        }        
        

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position = eventData.position - (Vector2)background.position;
            position = Vector2.ClampMagnitude(position, background.sizeDelta.x / 2);

            handle.anchoredPosition = position;
            _inputVector = position / (background.sizeDelta.x / 2);
            
            if(_inputVector == Vector2.zero) return;
            _onMove?.Invoke(_inputVector);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!eventData.pointerId.Equals(_joystickFingerId)) _joystickFingerId = eventData.pointerId;
            OnDrag(eventData);
        }
        
        public void RegisterActionMove(Action<Vector2> action)
        {
            _onMove = action;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId.Equals(_joystickFingerId)) _joystickFingerId = -1;
            handle.anchoredPosition = Vector2.zero;
            _inputVector = Vector2.zero;
            _onMove?.Invoke(_inputVector);
        }
        
        public bool IsMoving => _inputVector.magnitude > 0.1f;

        public int JoystickFingerId()
        {
            if (PlatformManager.Instance.IsTomko)
            {
                return _joystickFingerId;
            }

            return -1;
        }
    }
}
