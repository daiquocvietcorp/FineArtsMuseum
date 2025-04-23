using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnchorGrabHandler : MonoBehaviour
{
    public Transform anchor1;
    public Transform anchor2;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        StartCoroutine(SwitchAnchorDelayed(anchor2));
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        StartCoroutine(SwitchAnchorDelayed(anchor1));
    }

    private IEnumerator SwitchAnchorDelayed(Transform targetAnchor)
    {
        yield return null; // đợi 1 frame

        transform.SetParent(targetAnchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}