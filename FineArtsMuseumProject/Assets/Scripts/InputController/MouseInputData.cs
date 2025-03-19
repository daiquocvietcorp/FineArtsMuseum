using UnityEngine;

namespace InputController
{
    [CreateAssetMenu(fileName = "MouseInputData", menuName = "InputController/MouseInputData")]
    public class MouseInputData : ScriptableObject
    {
        [field: SerializeField] public float View3RdGoToPointLimitDistance { get; private set; } = 5f;
        [field: SerializeField] public float View1StGoToPointDistance { get; private set; } = 3f;
    }
}
