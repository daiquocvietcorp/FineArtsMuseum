using InputController;
using UnityEngine;

namespace Camera
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private CameraFollowData data;
        
        private float _currentYaw;
        private float _currentPitch;

        private float _mouseX;
        private float _mouseY;

        private void Start()
        {
            if(player) return;
            var angles = transform.eulerAngles;
            _currentYaw = angles.y;
            _currentPitch = angles.x;
        }

        private void Update()
        {
            UpdateCameraPosition();
            
            if (!MouseInput.Instance.IsHold) return;
            
            var mouseX = Input.GetAxis("Mouse X") * data.Sensitivity;
            var mouseY = -Input.GetAxis("Mouse Y") * data.Sensitivity;

            _currentYaw += mouseX;
            _currentPitch = Mathf.Clamp(_currentPitch + mouseY, data.MinPitch, data.MaxPitch);
        }

        private void UpdateCameraPosition()
        {
            if (!player) return;

            var rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
            var targetPosition = player.position + Vector3.up * data.Height;
            var position = targetPosition - (rotation * Vector3.forward * data.Distance);
        
            transform.position = position;
            transform.LookAt(targetPosition);
        }
    }
}
