using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class SliderThumb : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
    {
        private bool _isDragging = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            Debug.Log("Pointer Down");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                // Bạn có thể xử lý liên tục trong lúc kéo ở đây nếu muốn
                Debug.Log("Dragging...");
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDragging)
            {
                _isDragging = false;
                Debug.Log("Pointer Up");
            }
        }
    }
}
