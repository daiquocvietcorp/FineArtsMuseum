using InputController;
using Trigger;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ArcSlider : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public RectTransform thumb;
        public RectTransform dragArea;
        public Image fillArea;
        public float radius = 422f; // bán kính hình tròn
        [Range(0f, 1f)] public float t = 0.5f;

        // Cung từ trái → đỉnh → phải: 180° đến 0°
        public float startAngle = 180f; 
        public float endAngle = 0f;
    
        private float _previousT = 0f;
        private bool _isDragging = false;
        private float _currentOriginalScale;

        private bool _isChange = false;

        void Start()
        {
            UpdateThumbPosition();
        }
        
        public void SetValue(float value)
        {
            _currentOriginalScale = value;
            t = value;
            UpdateThumbPosition();
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dragArea, eventData.position, eventData.pressEventCamera, out var localPos
            );

            var angle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;

            // Giữ góc trong khoảng từ 180° đến 0° (tức là cung phía trên)
            angle = Mathf.Clamp(angle, endAngle, startAngle);
            t = Mathf.InverseLerp(startAngle, endAngle, angle);

            UpdateThumbPosition();
        }

        void UpdateThumbPosition()
        {
            var angle = Mathf.Lerp(startAngle, endAngle, t) * Mathf.Deg2Rad;
            var x = Mathf.Cos(angle) * radius;
            var y = Mathf.Sin(angle) * radius;
            thumb.anchoredPosition = new Vector2(x, y);
            fillArea.fillAmount = t;
            if (!Mathf.Approximately(t, _previousT))
            {
                _previousT = t;
                OnValueChanged(t);
            }
        }

        private void OnValueChanged(float f)
        {
            PaintingDetailManager.Instance.ZoomPainting(f);
            AntiqueManager.Instance.ZoomAntique(f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isChange = false;
            if (!_isDragging) return;
            _isDragging = false;
            MouseInput.Instance.SetSliderDrag(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isChange = true;
            _isDragging = true;
            MouseInput.Instance.SetSliderDrag(true);
        }

        public void ResetSlider()
        {
            t = _currentOriginalScale;
        }
        
        public bool IsChangeValue()
        {
            return _isChange;
        }
    }
}