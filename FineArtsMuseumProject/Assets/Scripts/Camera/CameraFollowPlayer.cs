using System;
using System.Collections;
using InputController;
using UnityEngine;

namespace Camera
{
    public class CameraFollowPlayer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private CameraFollowData data;
        [SerializeField] private DirectionFirstView directionFirstView;
        
        private float _currentYaw;
        private float _currentPitch;
        
        private bool _isFirstPerson;
        private bool _isActive;
        
        private Transform _currentTarget;
        private Coroutine _changeViewCoroutine;

        private void Awake()
        {
            data.Distance = data.View3RdPerson.Distance;
            data.Height = data.View3RdPerson.Height;
            _isActive = true;
            
            directionFirstView.DisableDirectionFirstView();
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
            directionFirstView.SetPosition(transform);
            
            if (!MouseInput.Instance.IsHold) return;
            _isActive = true;
            
#if UNITY_EDITOR || UNITY_STANDALONE
            
            var mouseX = Input.GetAxis("Mouse X") * data.Sensitivity;
            var mouseY = -Input.GetAxis("Mouse Y") * data.Sensitivity;
            _currentYaw += mouseX;
            _currentPitch = Mathf.Clamp(_currentPitch + mouseY, data.MinPitch, data.MaxPitch);
            
#else
            float mouseX = 0f;
            float mouseY = 0f;
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
            if(!_isActive)
            {
                var direction = _currentTarget.position - player.position;
                var targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                _currentYaw = Mathf.LerpAngle(_currentYaw, targetYaw, Time.deltaTime * data.Sensitivity);
                
                if (Mathf.Abs(Mathf.DeltaAngle(_currentYaw, targetYaw)) < 0.5f)
                {
                    _isActive = true;
                }
            }
            
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
            if(_isFirstPerson) return;
            MouseInput.Instance.ChangeView(_isFirstPerson);
            _isFirstPerson = true;
            
            if(_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            _changeViewCoroutine = StartCoroutine(ChangeView(data.View3RdPerson, data.View1StPerson));
            directionFirstView.EnableDirectionFirstView();
        }

        private IEnumerator ChangeView(CameraFollowDistance dataView3RdPerson, CameraFollowDistance dataView1StPerson)
        {
            const float duration = 0.5f;
            var time = 0f;
            
            while (time < duration)
            {
                time += Time.deltaTime;
                var t = time / duration;
                data.Distance = Mathf.Lerp(dataView3RdPerson.Distance, dataView1StPerson.Distance, t);
                data.Height = Mathf.Lerp(dataView3RdPerson.Height, dataView1StPerson.Height, t);
                yield return null;
            }
        }

        public void SetThirdPersonView()
        {
            if(!_isFirstPerson) return;
            MouseInput.Instance.ChangeView(_isFirstPerson);
            _isFirstPerson = false;
            
            if(_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            _changeViewCoroutine = StartCoroutine(ChangeView(data.View1StPerson, data.View3RdPerson));
            directionFirstView.DisableDirectionFirstView();
        }

        public void RotateCamera(Transform target)
        {
            if(target == null) return;
            _currentTarget = target;
            _isActive = false;
            //UpdateCameraPositionTmp();
        }
    }
}
