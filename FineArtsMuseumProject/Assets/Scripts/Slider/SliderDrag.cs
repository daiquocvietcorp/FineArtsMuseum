using System;
using InputController;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Slider
{
    public class SliderDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [field: SerializeField] private bool isRealTime = false;
        
        private Vector2 _startTouchPosition;
        private Vector2 _endTouchPosition;
        private bool _isSwiping = false;
        
        private Action<int> _changePage;
        private Action<bool> _clickAction;
        
        private BoxCollider _boxCollider;
        private bool _isHoldInputPrevious;

        private void Start()
        {
            _boxCollider = GetComponent<BoxCollider>();
            if(!isRealTime) return;
            if (_boxCollider)
                _boxCollider.enabled = false;
        }

        private void Update()
        {
            if(!isRealTime) return;
            if (MouseInput.Instance.IsHold)
            {
                if(_isHoldInputPrevious) return;
                _boxCollider.enabled = false;
                _isHoldInputPrevious = true;
                return;
            }
            
            if(!_isHoldInputPrevious) return;
            _boxCollider.enabled = true;
            _isHoldInputPrevious = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _startTouchPosition = eventData.position;
            _isSwiping = true;
        }

        public void FakePointerDown()
        {
            MouseInput.Instance.SetIsDragImage(true);
            _clickAction?.Invoke(false);
        }
        public void OnDrag(PointerEventData eventData)
        {
            MouseInput.Instance.SetIsDragImage(true);
            _clickAction?.Invoke(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isSwiping) return;

            _endTouchPosition = eventData.position;
            var swipeDistance = _endTouchPosition.x - _startTouchPosition.x;

            if (Mathf.Abs(swipeDistance) > 50)
            {
                if (swipeDistance > 0)
                {
                    _changePage?.Invoke(1);
                }
                else
                {
                    _changePage?.Invoke(-1);
                }
            }
            else
            {
                return;
            }

            _isSwiping = false;
            _clickAction?.Invoke(true);
            MouseInput.Instance.SetIsDragImage(false);
        }

        public void RegisterDragAction(Action<int> changePage)
        {
            _changePage = changePage;
            _boxCollider.enabled = true;
        }
        
        public void RegisterClickAction(Action<bool> clickAction)
        {
            _clickAction = clickAction;
        }
    }
}
