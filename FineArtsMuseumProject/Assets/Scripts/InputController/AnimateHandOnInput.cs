using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;
    void Update()
    {
        float triggerSelect = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger",triggerSelect);
        float trigerGrip = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip",trigerGrip);
    }
}
