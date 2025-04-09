using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class StartButtonController : MonoBehaviour
{
    public UIStart UIStart;
    
    public void OnAnimationComplete()
    {
        UIStart.EnterMain(); }
}
