using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private bool isEditorMode;
        
        [SerializeField] private LayerMask blockingLayerMask;
        [SerializeField] private float collisionOffset = 0.2f;
        
        [SerializeField] private List<CameraDefault> cameraDefaultList;
        
        private Dictionary<int, CameraDefault> _cameraDefaultDictionary;
        
        private float _currentYaw;
        private float _currentPitch;
        private float _currentAreaYaw;
        private float _currentAreaPitch;
        
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
            if (SceneLog.IsFirstView)
            {
                data.Distance = data.View1StPerson.Distance;
                data.Height = data.View1StPerson.Height;
            }
            else
            {
                data.Distance = data.View3RdPerson.Distance;
                data.Height = data.View3RdPerson.Height;
            }
            _isActive = true;
            _joystickDirection = Vector2.zero;
            directionFirstView.DisableDirectionFirstView();
            _cameraDefaultDictionary = new Dictionary<int, CameraDefault>();
            foreach (var cameraDefault in cameraDefaultList)
            {
                _cameraDefaultDictionary.TryAdd(cameraDefault.sceneId, cameraDefault);
            }
            
            SetCameraFirstView();
                
        }

        private void Start()
        {
            if(!player) return;
            var angles = transform.eulerAngles;
            _currentYaw = angles.y;
            _currentPitch = angles.x;
            _isFirstPerson = SceneLog.IsFirstView;
        }

        public void RegisterRotationAction()
        {
            JoystickRotationInput.Instance.RegisterActionRotate(UpdateJoystickDirection);
        }

        private void SetCameraFirstView()
        {
            if (isEditorMode) return;
            if (SceneLog.IsFirstScene)
            {
                _currentTargetPosition = data.DefaultPosition;
                _currentTargetRotation = Quaternion.Euler(data.DefaultRotation);
                transform.position = data.DefaultPosition;
                transform.rotation = Quaternion.Euler(data.DefaultRotation);
            }
            else
            {
                if(!_cameraDefaultDictionary.TryGetValue(SceneLog.PreviousSceneId, out var cameraDefault)) return;
                _currentTargetRotation = Quaternion.Euler(cameraDefault.rotation);
                transform.rotation = Quaternion.Euler(cameraDefault.rotation);

                if (SceneLog.IsFirstView) return;
                transform.position = cameraDefault.position;
                _currentTargetPosition = cameraDefault.position;
            }
        }
        
        public void SetCamera(Vector3 position, Vector3 rotation)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
            _currentTargetPosition = position;
            _currentTargetRotation = Quaternion.Euler(rotation);
        }

        private void Update()
        {
            if(PlatformManager.Instance.IsVR) return;
            //UpdateCameraPosition();
            UpdateCameraPositionWithData();
            directionFirstView.SetPosition(transform);
            
            float mouseX = 0;
            float mouseY = 0;

            if (PlatformManager.Instance.IsStandalone)
            {
                if (!MouseInput.Instance.IsHold) return;
                _isActive = true;
                
                mouseX = Input.GetAxis("Mouse X") * data.Sensitivity;
                mouseY = -Input.GetAxis("Mouse Y") * data.Sensitivity;
                
                _currentYaw += mouseX;
                _currentPitch = Mathf.Clamp(_currentPitch + mouseY, data.MinPitch, data.MaxPitch);

                _currentAreaYaw += mouseX;
                _currentAreaPitch = Mathf.Clamp(_currentAreaPitch + mouseY, data.MinPitch, data.MaxPitch);
            }
            
            if (PlatformManager.Instance.IsTomko)
            {
                if (!MouseInput.Instance.IsHold) return;
                _isActive = true;
                
                if (Input.touchCount > 0)
                {
                    var touch = Input.GetTouch(MouseInput.Instance.GetCurrentFingerId());
                    if (touch.phase == TouchPhase.Moved)
                    {
                        var delta = touch.deltaPosition;
                        mouseX = delta.x * data.Sensitivity * 0.1f;
                        mouseY = delta.y * data.Sensitivity * 0.1f;
                    }
                }
                
                _currentYaw += mouseX;
                _currentPitch = Mathf.Clamp(_currentPitch - mouseY, data.MinPitch, data.MaxPitch);
                
                _currentAreaYaw += mouseX;
                _currentAreaPitch = Mathf.Clamp(_currentAreaPitch - mouseY, data.MinPitch, data.MaxPitch);
            }

            if (PlatformManager.Instance.IsCloud || PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsTomko || PlatformManager.Instance.IsWebGL)
            {
                //if(_joystickDirection.magnitude < 0.1f) return;
                if(_joystickDirection == Vector2.zero) return;
                
                _isActive = true;
                
                mouseX = _joystickDirection.x * data.Sensitivity;
                mouseY = -_joystickDirection.y * data.Sensitivity;
                
                _currentYaw += mouseX;
                _currentPitch = Mathf.Clamp(_currentPitch - mouseY, data.MinPitch, data.MaxPitch);
                
                _currentAreaYaw += mouseX;
                _currentAreaPitch = Mathf.Clamp(_currentAreaPitch - mouseY, data.MinPitch, data.MaxPitch);
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
                
                if (Mathf.Abs(Mathf.DeltaAngle(_currentYaw, targetYaw)) < 0.1f
                    && Mathf.Abs(Mathf.DeltaAngle(_currentPitch, targetPitch)) < 0.1f)
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
                _currentAreaYaw = transform.eulerAngles.y;
                var rawPitch = transform.eulerAngles.x;
                _currentAreaPitch = rawPitch > 180f ? rawPitch - 360f : rawPitch;
                return;
            }

            if (!_isLockFollowView)
            {
                var targetPosition = player.position + Vector3.up * data.Height;
                var rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
                var desiredDistance = data.Distance;
                var desiredPos = targetPosition - (rotation * Vector3.forward * desiredDistance);
                var direction = (desiredPos - targetPosition).normalized;
                var maxDistance = Vector3.Distance(targetPosition, desiredPos);

                if (Physics.Raycast(targetPosition, direction, out var hit, maxDistance, blockingLayerMask))
                {
                    var newDistance = hit.distance - collisionOffset;
                    newDistance = Mathf.Max(newDistance, 0.5f);
                    desiredPos = targetPosition - (rotation * Vector3.forward * newDistance);
                }
                
                transform.position = desiredPos;
                if (!_isFirstPerson)
                {
                    transform.LookAt(targetPosition);
                }
                else
                {
                    transform.rotation = rotation;
                }
            }
            else
            {
                var rotation = Quaternion.Euler(_currentAreaPitch, _currentAreaYaw, 0);
                transform.rotation = rotation;
                
                //follow player
                var targetPosition = player.position + Vector3.up * data.Height;
                var desiredDistance = data.Distance;
                var desiredPos = targetPosition - (rotation * Vector3.forward * desiredDistance);
                var direction = (desiredPos - targetPosition).normalized;
                var maxDistance = Vector3.Distance(targetPosition, desiredPos);
                if (Physics.Raycast(targetPosition, direction, out var hit, maxDistance, blockingLayerMask))
                {
                    var newDistance = hit.distance - collisionOffset;
                    newDistance = Mathf.Max(newDistance, 0.5f);
                    desiredPos = targetPosition - (rotation * Vector3.forward * newDistance);
                }
                
                transform.position = desiredPos;
            }
        }

        private void UpdateCameraInArea()
        {
            
        }
        
        public void SetFirstPersonView(float distance = -1, float height = -1)
        {
            SceneLog.IsFirstView = true;
            //if(_isFirstPerson) return;
            _isFirstPerson = true;
            MouseInput.Instance.ChangeView(_isFirstPerson);
            CharacterManager.Instance.HideCharacterSkin();
            
            if(_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            if(!Mathf.Approximately(distance, -1) && !Mathf.Approximately(height, -1))
            {
                var dataView = new CameraFollowDistance()
                {
                    Distance = distance,
                    Height = height
                };
                _changeViewCoroutine = StartCoroutine(ChangeView(data.View3RdPerson, dataView));
            }
            else
            {
                _changeViewCoroutine = StartCoroutine(ChangeView(data.View3RdPerson, data.View1StPerson));
            }
            
            directionFirstView.EnableDirectionFirstView();
        }
        
        public bool IsFirstPerson => _isFirstPerson;
        public bool IsLocked => _isLockFollowView;

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
            SceneLog.IsFirstView = false;
            //if(!_isFirstPerson) return;
            _isFirstPerson = false;
            MouseInput.Instance.ChangeView(_isFirstPerson);
            CharacterManager.Instance.ShowCharacterSkin();
            
            if(_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            if(!Mathf.Approximately(_exitPaintingCameraDistance, -1) && !Mathf.Approximately(_exitPaintingCameraHeight, -1))
            {
                var dataView = new CameraFollowDistance()
                {
                    Distance = _exitPaintingCameraDistance,
                    Height = _exitPaintingCameraHeight
                };
                _exitPaintingCameraDistance = -1;
                _exitPaintingCameraHeight = -1;
                _changeViewCoroutine = StartCoroutine(ChangeView(dataView, data.View3RdPerson));
            }
            else
            {
                _changeViewCoroutine = StartCoroutine(ChangeView(data.View1StPerson, data.View3RdPerson));
            }

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
            if(isEditorMode) return;
            _currentTargetPosition = position;
            _currentTargetRotation = rotation;
            _isActive = false;
        }

        public void ResetCameraInArea()
        {
            if(isEditorMode) return;
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

        public void EnterArea(float distance = -1, float height = -1)
        {
            if(isEditorMode) return;
            _isLockFollowView = true;
            //Nếu là góc nhìn thứ nhất thì bỏ qua
            if (_isFirstPerson)
            {
                _isExitFirstView = true;
                return;
            }
            _isExitFirstView = false;
            
            _exitPaintingCameraDistance = distance;
            _exitPaintingCameraHeight = height;
            
            SetFirstPersonView(distance, height);
            if (!_isExitFirstView)
            {
                _isFirstPerson = false;
            }

            /*if (_changeViewCoroutine != null)
            {
                StopCoroutine(_changeViewCoroutine);
            }
            
            _changeViewCoroutine = StartCoroutine(ChangeView(distance, height));
            if(!_isFirstPerson) directionFirstView.EnableDirectionFirstView();*/
        }

        public void ExitArea()
        {
            if(isEditorMode) return;
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
    
    [Serializable]
    public class CameraDefault
    {
        public int sceneId;
        public Vector3 position;
        public Vector3 rotation;
    }
}
