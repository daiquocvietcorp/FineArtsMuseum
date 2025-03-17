using UnityEngine;

namespace Player
{
    public class CharacterStateMachine : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private ICharacterAnimation _currentState;
        private Vector3 _jumpDirection;
        
        [field: SerializeField] private Animator animator;
        [field: SerializeField] private CharacterData data;
        [field: SerializeField] private Transform cameraTransform;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _currentState = new CharacterIdleState();
            _currentState.EnterState(this);
            
            //stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
        }

        public void PlayAnimation(string animationName)
        {
            animator.Play(animationName);
        }

        private void Update()
        {
            _currentState.UpdateState(this);
        }

        private void FixedUpdate()
        {
            _currentState.FixedUpdateState(this);
            //StepClimb();
        }

        public void SwitchState(ICharacterAnimation newState)
        {
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
            
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;
                
            forward.y = 0;
            right.y = 0;
                
            forward.Normalize();
            right.Normalize();
                
            var desiredMoveDirection = forward * moveDirection.z + right * moveDirection.x;
            var toRotation = Quaternion.LookRotation(desiredMoveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, data.RotationSpeed * Time.deltaTime);
                
            _rigidbody.MovePosition(transform.position + desiredMoveDirection * (data.MovementSpeed * Time.fixedDeltaTime));
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
                var forward = cameraTransform.forward;
                var right = cameraTransform.right;
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
            
            Vector3 jumpForce = _jumpDirection * data.JumpForce + Vector3.up * data.JumpForce;
            _rigidbody.velocity = jumpForce;
        }

        public bool IsJump(float jumpTime)
        {
            return jumpTime >= data.MinJumpTime + data.WaitingJumpTime;
        }
    }
}
