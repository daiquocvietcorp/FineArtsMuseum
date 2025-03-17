namespace Player
{
    public interface ICharacterAnimation
    {
        void EnterState(CharacterStateMachine character);
        void UpdateState(CharacterStateMachine character);
        void ExitState(CharacterStateMachine character);
        void FixedUpdateState(CharacterStateMachine character);
    }
}
