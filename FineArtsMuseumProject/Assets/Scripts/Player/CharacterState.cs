using UnityEngine;

namespace Player
{
    public class CharacterIdleState : ICharacterAnimation
    {
        public void EnterState(CharacterStateMachine character)
        {
            character.PlayAnimation("Idle");
        }

        public void UpdateState(CharacterStateMachine character)
        {
            if(character.IsMoving())
            {
                character.SwitchState(new CharacterWalkState());
            }
            
            if(Input.GetKeyDown(KeyCode.Space))
            {
                character.SwitchState(new CharacterJumpState());
            }
        }

        public void ExitState(CharacterStateMachine character)
        {
            //throw new System.NotImplementedException();
        }

        public void FixedUpdateState(CharacterStateMachine character)
        {
            
        }
    }
    
    public class CharacterWalkState : ICharacterAnimation
    {
        public void EnterState(CharacterStateMachine character)
        {
            character.PlayAnimation("Walk");
        }

        public void UpdateState(CharacterStateMachine character)
        {
            if(!character.IsMoving() && !character.IsUsingTouch && !character.IsUsingJoystick)
            {
                character.SwitchState(new CharacterIdleState());
            }
            
            if(Input.GetKeyDown(KeyCode.Space))
            {
                character.SwitchState(new CharacterJumpState());
            }
        }

        public void ExitState(CharacterStateMachine character)
        {
            //throw new System.NotImplementedException();
        }

        public void FixedUpdateState(CharacterStateMachine character)
        {
            if(!character.IsMoving()) return;
            character.MoveCharacter();
            //character.StepClimb();
        }
    }
    
    public class CharacterJumpState : ICharacterAnimation
    {
        private float _jumpTime;
        public void EnterState(CharacterStateMachine character)
        {
            //character.PlayAnimation(character.IsMoving() ? "WalkJump" : "Jump");
            character.PlayAnimation("Jump");
            character.PrepareJump();
            _jumpTime = 0;
        }

        public void UpdateState(CharacterStateMachine character)
        {
            _jumpTime += Time.deltaTime;
            
            if (character.IsGrounded() && character.IsJump(_jumpTime))
            {
                if(!character.IsMoving())
                {
                    character.SwitchState(new CharacterIdleState());
                    return;
                }
                character.SwitchState(new CharacterWalkState());
            }
        }

        public void ExitState(CharacterStateMachine character)
        {
            //throw new System.NotImplementedException();
        }

        public void FixedUpdateState(CharacterStateMachine character)
        {
            //throw new System.NotImplementedException();
        }
        
    }
}
