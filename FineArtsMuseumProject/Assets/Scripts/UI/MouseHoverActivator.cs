using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHoverActivator : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject selectableObject;

    private void Start()
    {
        if (selectableObject != null)
        {
            selectableObject.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if (selectableObject != null)
        {
            selectableObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (selectableObject != null)
        {
            selectableObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectableObject != null)
        {
            selectableObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectableObject != null)
        {
            selectableObject.SetActive(false);
        }
    }
}