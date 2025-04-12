using System;
using System.Collections.Generic;
using Camera;
using InputController;
using LayerMasks;
using UnityEngine;

namespace Player
{
    public class CharacterStateMachine : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private ICharacterAnimation _currentState;
        private Vector3 _jumpDirection;
        
        private Vector3 _touchPosition;
        private bool _isUsingTouch;
        private bool _isActive;
        
        private bool _isUsingJoystick;
        private Vector2 _joystickDirection;
        
        [field: SerializeField] private Animator animator;
        [field: SerializeField] private CharacterData data;
        [field: SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [field: SerializeField] private List<CharacterDefault> characterDefaults;

        private Dictionary<int, CharacterDefault> _characterDefaultDictionary;
        private UnityEngine.Camera _cameraMain;
        private bool _isStarted;

        public bool IsUsingTouch => _isUsingTouch;
        public bool IsUsingJoystick => _isUsingJoystick;

        public void StartCharacter()
        {
            _isStarted = true;
        }
        
        private void Awake()
        {
            _isActive = true;
            _isStarted = false;
            _characterDefaultDictionary = new Dictionary<int, CharacterDefault>();
            foreach (var characterDefault in characterDefaults)
            {
                _characterDefaultDictionary.TryAdd(characterDefault.sceneId, characterDefault);
            }
            SetCharacterDefault();
            if (PlatformManager.Instance.IsVR)
            {
                gameObject.SetActive(false);
            }
        }

        private void SetCharacterDefault()
        {
            if(!SceneLog.IsFirstScene)
            {
                if(!_characterDefaultDictionary.TryGetValue(SceneLog.PreviousSceneId, out var dDefault)) return;
                transform.position = dDefault.position;
                transform.rotation = Quaternion.Euler(dDefault.rotation);
                return;
            }
            transform.position = data.DefaultPosition;
            transform.rotation = Quaternion.Euler(data.DefaultRotation);
        }

        private void Start()
        {
            _cameraMain = CameraManager.Instance.mainCamera;
            _rigidbody = GetComponent<Rigidbody>();
            _currentState = new CharacterIdleState();
            _currentState.EnterState(this);
            
            //stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);

            if (PlatformManager.Instance.IsCloud)
            {
                MouseInput.Instance.RegisterClick(MoveCharacterByGoToPointCloud);
            }
            else
            {
                MouseInput.Instance.RegisterClick(MoveCharacter);
            }
        }
        
        public void RegisterJoystickAction()
        {
            JoystickInput.Instance.RegisterActionMove(MoveCharacter);
        }

        public void PlayAnimation(string animationName)
        {
            animator.Play(animationName);
        }

        private void Update()
        {
            if(!_isStarted) return;
            if (!_isActive) return;
            _currentState.UpdateState(this);

            if (_isUsingTouch)
            {
                if (IsMoving())
                {
                    _isUsingTouch = false;
                    return;
                }

                MoveByTouch();
            }

            if (_isUsingJoystick)
            {
                MoveByJoystick();
            }
        }

        private void FixedUpdate()
        {
            if (!_isActive) return;
            _currentState.FixedUpdateState(this);
            //StepClimb();
        }

        public void SwitchState(ICharacterAnimation newState)
        {
            if (PlatformManager.Instance.IsVR) return;
            _currentState.ExitState(this);
            _currentState = newState;
            _currentState.EnterState(this);
        }

        public bool IsMoving()
        {
            return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        }

        public bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1.1f);
        }

        public void MoveCharacter()
        {
            var moveX = Input.GetAxis("Horizontal");
            var moveZ = Input.GetAxis("Vertical");
            
            var moveDirection = new Vector3(moveX, 0, moveZ).normalized;

            if (!(moveDirection.magnitude >= 0.1f)) return;
            
            var forward = _cameraMain.transform.forward;
            var right = _cameraMain.transform.right;
                
            forward.y = 0;
            right.y = 0;
                
            forward.Normalize();
            right.Normalize();
                
            var desiredMoveDirection = forward * moveDirection.z + right * moveDirection.x;
            var toRotation = Quaternion.LookRotation(desiredMoveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, data.RotationSpeed * Time.deltaTime);
            _rigidbody.MovePosition(transform.position + desiredMoveDirection * (data.MovementSpeed * Time.fixedDeltaTime));
        }
        
        private void MoveByJoystick()
        {
            if (!(_joystickDirection.magnitude > 0.1f)) return;
            var moveDirection = new Vector3(_joystickDirection.x, 0, _joystickDirection.y).normalized;
            var forward = _cameraMain.transform.forward;
            var right = _cameraMain.transform.right;
            
            forward.y = 0;
            right.y = 0;
            
            forward.Normalize();
            right.Normalize();
            
            var desiredMoveDirection = forward * moveDirection.z + right * moveDirection.x;
            var toRotation = Quaternion.LookRotation(desiredMoveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, data.RotationSpeed * Time.deltaTime);
            //transform.position = Vector3.MoveTowards(transform.position, transform.position + desiredMoveDirection, data.MovementSpeed * Time.deltaTime);
            _rigidbody.MovePosition(transform.position + desiredMoveDirection * (data.MovementSpeed * Time.fixedDeltaTime));
        }

        private void MoveCharacter(Vector3 position)
        {
            if(MouseInput.Instance.IsPointerOverUI()) return;
            
            var ray = _cameraMain.ScreenPointToRay(position);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerManager.Instance.groundLayer)) return;
            
            MoveCharacterByGoToPointCloud(hit.point);
        }

        private void MoveCharacterByGoToPointCloud(Vector3 position)
        {
            _touchPosition = position;
            _isUsingTouch = true;
            
            if (PlatformManager.Instance.IsMobile)
            {
                MouseInput.Instance.ShowGoToPointer(_touchPosition);
            }
            
            _currentState.ExitState(this);
            _currentState = new CharacterWalkState();
            _currentState.EnterState(this);
        }
        
        private void MoveByTouch()
        {
            if(_currentState is not CharacterWalkState) return;
            var direction = (_touchPosition - transform.position).normalized;
            if (direction.magnitude > 0.1f)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                //transform.position = Vector3.MoveTowards(transform.position, _touchPosition, data.MovementSpeed * Time.deltaTime);
                _rigidbody.MovePosition(transform.position + direction * (data.MovementSpeed * Time.fixedDeltaTime));
            }

            if (Vector3.Distance(transform.position, _touchPosition) < 0.1f)
            {
                _isUsingTouch = false;
            }
        }

        private void MoveCharacter(Vector2 direction)
        {
            if (direction.magnitude < 0.1f)
            {
                _isUsingJoystick = false;
                SwitchState(new CharacterIdleState());
            }
            else
            {
                _isUsingJoystick = true;
                _joystickDirection = direction;
                if (_currentState is CharacterWalkState) return;
                SwitchState(new CharacterWalkState());
            }
        }
        
        public void PrepareJump()
        {
            Invoke(nameof(Jump), data.WaitingJumpTime);
        }

        public void Jump()
        {
            var moveX = Input.GetAxis("Horizontal");
            var moveZ = Input.GetAxis("Vertical");
            
            var moveInput = new Vector3(moveX, 0, moveZ).normalized;
            if (moveInput.magnitude >= 0.1f)
            {
                var forward = _cameraMain.transform.forward;
                var right = _cameraMain.transform.right;
                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();

                _jumpDirection = forward * moveInput.z + right * moveInput.x;
            }
            else
            {
                _jumpDirection = Vector3.zero;
            }
            
            var jumpForce = _jumpDirection * data.JumpForce + Vector3.up * data.JumpForce;
            _rigidbody.velocity = jumpForce;
        }

        public bool IsJump(float jumpTime)
        {
            return jumpTime >= data.MinJumpTime + data.WaitingJumpTime;
        }

        public void DisableCharacter()
        {
            _isActive = false;
            gameObject.SetActive(false);
        }
        
        public void EnableCharacter()
        {
            _isActive = true;
            gameObject.SetActive(true);
            SwitchState(new CharacterIdleState());
        }
        
        public void ShowCharacter()
        {
            skinnedMeshRenderer.enabled = true;
        }
        
        public void HideCharacter()
        {
            skinnedMeshRenderer.enabled = false;
        }
        
        public void SetCharacter(Vector3 position, Vector3 rotation)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    [Serializable]
    public class CharacterDefault
    {
        public int sceneId;
        public Vector3 position;
        public Vector3 rotation;
    }
}
