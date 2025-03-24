using UnityEngine;

namespace Camera
{
    public class DirectionFirstView : MonoBehaviour
    {
        //public void SetPosition
        [field: SerializeField] private float distance;
        [field: SerializeField] private float height;
        
        private bool _isActive;
        
        public void SetPosition(Transform target)
        {
            if (!_isActive) return;
            var fixedY = height;
            
            var newPosition = target.position + target.rotation * new Vector3(0, 0, distance);
            newPosition.y = fixedY;
            transform.position = newPosition;
            
            transform.rotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        }
        
        public void EnableDirectionFirstView()
        {
            _isActive = true;
            gameObject.SetActive(true);
        }
        
        public void DisableDirectionFirstView()
        {
            _isActive = false;
            gameObject.SetActive(false);
        }
    }
}
