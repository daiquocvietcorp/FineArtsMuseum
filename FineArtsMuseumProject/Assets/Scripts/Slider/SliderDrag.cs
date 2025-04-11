using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Slider
{
    public class SliderDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private Vector2 _startTouchPosition;
        private Vector2 _endTouchPosition;
        private bool _isSwiping = false;
        
        private Action<int> _changePage;
        private Action<bool> _clickAction;

        public void OnPointerDown(PointerEventData eventData)
        {
            _startTouchPosition = eventData.position;
            _isSwiping = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
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
        }

        public void RegisterDragAction(Action<int> changePage)
        {
            _changePage = changePage;
        }
        
        public void RegisterClickAction(Action<bool> clickAction)
        {
            _clickAction = clickAction;
        }
    }
}
