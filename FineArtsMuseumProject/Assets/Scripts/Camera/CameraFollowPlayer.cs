using System;
using System.Collections;
using DG.Tweening;
using InputController;
using Player;
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
        private Vector3 _currentTargetPosition;
        private Quaternion _currentTargetRotation;
        
        private Coroutine _changeViewCoroutine;
        
        private Vector2 _joystickDirection;
        private float _exitPaintingCameraDistance;
        private float _exitPaintingCameraHeight;
        private bool _isExitFirstView;
        private bool _isLockFollowView;

        private void Awake()
        {
            data.Distance = data.View3RdPerson.Distance;
            data.Height = data.View3RdPerson.Height;
            _isActive = true;
            _joystickDirection = Vector2.zero;
            
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

        public void RegisterRotationAction()
        {
            JoystickRotationInput.Instance.RegisterActionRotate(UpdateJoystickDirection);
        }

        private void Update()
        {
            if(PlatformManager.Instance.IsVR) return;
            //UpdateCameraPosition();
            UpdateCameraPositionWithData();
            directionFirstView.SetPosition(transform);
            
            float mouseX = 0;
            float mouseY = 0;

            if (PlatformManager.Instance.IsStandalone || PlatformManager.Instance.IsWebGL)
            {
                if (!MouseInput.Instance.IsHold) return;

                if (!_isLockFollowView)
                {
                    _isActive = true;
                }
                
                mouseX = Input.GetAxis("Mouse X") * data.Sensitivity;
                mouseY = -Input.GetAxis("Mouse Y") * data.Sensitivity;
                
                _currentYaw += mouseX;
                _currentPitch = Mathf.Clamp(_currentPitch + mouseY, data.MinPitch, data.MaxPitch);
            }
            
            if (PlatformManager.Instance.IsMobile && false)
            {
                if (!MouseInput.Instance.IsHold || JoystickInput.Instance.IsMoving) return;
                _isActive = true;
                
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
            }

            if (PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsMobile)
            {
                //if(_joystickDirection.magnitude < 0.1f) return;
                if(_joystickDirection == Vector2.zero) return;
                
                if (!_isLockFollowView)
                {
                    _isActive = true;
                }
                
                mouseX = _joystickDirection.x * data.Sensitivity;
                mouseY = _joystickDirection.y * data.Sensitivity;
                
                _currentYaw += mouseX;
                _currentPitch = Mathf.Clamp(_currentPitch - mouseY, data.MinPitch, data.MaxPitch);
            }
            
            /*_currentYaw += mouseX;
            _currentPitch = Mathf.Clamp(_currentPitch + mouseY, data.MinPitch, data.MaxPitch);*/
        }
        
        private void UpdateJoystickDirection(Vector2 direction)
        {
            _joystickDirection = direction;
        }

        private void UpdateCameraPosition()
        {
            if (!player) return;
            if(!_isActive)
            {
                /*transform.DODynamicLookAt(_currentTarget.position, 0.5f);
                
                _currentYaw = transform.eulerAngles.y;
                _currentPitch = -transform.eulerAngles.x;
                
                _isActive = true;
                return;*/
                var direction = _currentTarget.position - player.position;
                var targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                var horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
                var targetPitch = Mathf.Atan2(direction.y, horizontalDistance) * Mathf.Rad2Deg;
                
                _currentYaw = Mathf.LerpAngle(_currentYaw, targetYaw, Time.deltaTime * data.Sensitivity);
                _currentPitch = Mathf.LerpAngle(_currentPitch, -targetPitch/3f, Time.deltaTime * data.Sensitivity);
                
                if (Mathf.Abs(Mathf.DeltaAngle(_currentYaw, targetYaw)) < 0.5f
                    && Mathf.Abs(Mathf.DeltaAngle(_currentPitch, -targetPitch/3)) < 0.5f)
                {
                    _isActive = true;
                }
                
                /*var direction = _currentTarget.position - player.position;

                var targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                var horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
                var targetPitch = Mathf.Atan2(direction.y, horizontalDistance) * Mathf.Rad2Deg;

                _currentYaw = Mathf.LerpAngle(_currentYaw, targetYaw, Time.deltaTime * data.Sensitivity);
                _currentPitch = Mathf.LerpAngle(_currentPitch, targetPitch, Time.deltaTime * data.Sensitivity);

                transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);

                if (Mathf.Abs(Mathf.DeltaAngle(_currentYaw, targetYaw)) < 0.5f &&
                    Mathf.Abs(Mathf.DeltaAngle(_currentPitch, targetPitch)) < 0.5f)
                {
                    _isActive = true;
                }*/
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
        
        private void UpdateCameraPositionWithData()
        {
            if (!player) return;
            if(!_isActive)
            {
                transform.position = Vector3.Lerp(transform.position, _currentTargetPosition, Time.deltaTime * data.Sensitivity);
                transform.rotation = Quaternion.Lerp(transform.rotation, _currentTargetRotation, Time.deltaTime * data.Sensitivity);
                _currentYaw = transform.eulerAngles.y;
                var rawPitch = transform.eulerAngles.x;
                _currentPitch = rawPitch > 180f ? rawPitch - 360f : rawPitch;
                return;
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

        private void UpdateCameraInArea()
        {
            
        }
        
        public void SetFirstPersonView()
        {
            if(_isFirstPerson) return;
            _isFirstPerson = true;
            MouseInput.Instance.ChangeView(_isFirstPerson);
            CharacterManager.Instance.HideCharacterSkin();
            
            if(_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            _changeViewCoroutine = StartCoroutine(ChangeView(data.View3RdPerson, data.View1StPerson));
            directionFirstView.EnableDirectionFirstView();
        }
        
        public bool IsFirstPerson => _isFirstPerson;

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
            _isFirstPerson = false;
            MouseInput.Instance.ChangeView(_isFirstPerson);
            CharacterManager.Instance.ShowCharacterSkin();
            
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

        public void SetCameraData(Vector3 position, Quaternion rotation)
        {
            _currentTargetPosition = position;
            _currentTargetRotation = rotation;
            _isActive = false;
        }
        
        private IEnumerator ChangeView(float distance, float height)
        {
            const float duration = 0.5f;
            var time = 0f;
            
            while(time < duration)
            {
                time += Time.deltaTime;
                var t = time / duration;
                data.Distance = Mathf.Lerp(data.Distance, distance, t);
                data.Height = Mathf.Lerp(data.Height, height, t);
                yield return null;
            }
        }

        public void EnterPainting(float distance, float height)
        {
            _isLockFollowView = true;
            //Nếu là góc nhìn thứ nhất thì bỏ qua
            if (_isFirstPerson)
            {
                _isExitFirstView = true;
                return;
            }
            _isExitFirstView = false;
            
            _exitPaintingCameraDistance = data.Distance;
            _exitPaintingCameraHeight = data.Height;
            
            SetFirstPersonView();

            /*if (_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            _changeViewCoroutine = StartCoroutine(ChangeView(distance, height));
            if(!_isFirstPerson) directionFirstView.EnableDirectionFirstView();*/
        }

        public void ExitPainting()
        {
            _isLockFollowView = false;
            _isActive = true;
            //Nếu ra ngoài mà là góc nhìn thứ nhất thì bỏ qua
            if(_isExitFirstView) return;
            SetThirdPersonView();
            
            /*if (_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            _changeViewCoroutine = StartCoroutine(ChangeView(_exitPaintingCameraDistance, _exitPaintingCameraHeight));
            if(!_isFirstPerson) directionFirstView.DisableDirectionFirstView();*/
        }
    }
}
