using System;
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
        
        private bool _isFirstPerson;

        private void Awake()
        {
            data.Distance = data.View3RdPerson.Distance;
            data.Height = data.View3RdPerson.Height;
        }

        private void Start()
        {
            if(!player) return;
            var angles = transform.eulerAngles;
            _currentYaw = angles.y;
            _currentPitch = angles.x;
            _isFirstPerson = false;
        }

        private void Update()
        {
            UpdateCameraPosition();
            
            if (!MouseInput.Instance.IsHold) return;
            
#if UNITY_EDITOR || UNITY_STANDALONE
            
            var mouseX = Input.GetAxis("Mouse X") * data.Sensitivity;
            var mouseY = -Input.GetAxis("Mouse Y") * data.Sensitivity;
            _currentYaw += mouseX;
            _currentPitch = Mathf.Clamp(_currentPitch + mouseY, data.MinPitch, data.MaxPitch);
            
#else
            
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    var delta = touch.deltaPosition;
                    mouseX = delta.x * data.Sensitivity * 0.1f;
                    mouseY = delta.y * data.Sensitivity * 0.1f;
                }
            }
            
            _currentYaw += mouseX;
            _currentPitch = Mathf.Clamp(_currentPitch - mouseY, data.MinPitch, data.MaxPitch);
            
#endif
            
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
            
            if(!_isFirstPerson)
            {
                transform.LookAt(targetPosition);
                return;
            }
            
            transform.rotation = rotation;
        }
        
        public void SetFirstPersonView()
        {
            data.Distance = data.View1StPerson.Distance;
            data.Height = data.View1StPerson.Height;
            _isFirstPerson = true;
            
            MouseInput.Instance.ChangeView(_isFirstPerson);
        }
        
        public void SetThirdPersonView()
        {
            data.Distance = data.View3RdPerson.Distance;
            data.Height = data.View3RdPerson.Height;
            _isFirstPerson = false;
            
            MouseInput.Instance.ChangeView(_isFirstPerson);
        }
    }
}
