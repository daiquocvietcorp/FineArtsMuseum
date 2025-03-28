using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    public class GoToPointCloud : MonoBehaviour, IPointerDownHandler
    {
        private Action<Vector3> _onClick;
        
        public void RegisterActionClick(Action<Vector3> action)
        {
            _onClick = action;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _onClick?.Invoke(transform.position);
        }
        
        public Vector3 GetPointOnPointCloud()
        {
            return transform.position;
        }
    }
}
