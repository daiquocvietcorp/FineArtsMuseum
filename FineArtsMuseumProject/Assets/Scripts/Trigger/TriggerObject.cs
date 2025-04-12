using System;
using System.Collections;
using Trigger;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerObject : MonoBehaviour, IPointerDownHandler
{
    public string antiqueID;
    
    //private MouseHoverActivator _hoverEffect;
    private Coroutine _playAnimationCoroutine;

    private void Start()
    {
        //_hoverEffect = GetComponent<MouseHoverActivator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (antiqueID != null || antiqueID != "")
        {
            if (PlatformManager.Instance.IsMobile || PlatformManager.Instance.IsTomko ||
                PlatformManager.Instance.IsCloud)
            {
                if(_playAnimationCoroutine != null) 
                    StopCoroutine(_playAnimationCoroutine);
                _playAnimationCoroutine = StartCoroutine(PlayObjectAnimation());
            }
            else
            {
                AntiqueManager.Instance.EnableAntiqueDetail(antiqueID);
            }
        }
    }

    private IEnumerator PlayObjectAnimation()
    {
        //_hoverEffect.FakePointerEnter();
        //yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0f);
        //_hoverEffect.FakePointerExit();
        AntiqueManager.Instance.EnableAntiqueDetail(antiqueID);
    }

    public void FakePointerDown()
    {
        if (antiqueID != null || antiqueID != "")
        {
            AntiqueManager.Instance.EnableAntiqueDetail(antiqueID);
        }
    }
}