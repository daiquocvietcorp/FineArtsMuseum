using Trigger;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerObject : MonoBehaviour, IPointerDownHandler
{
    [field: SerializeField]private string antiqueID;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (antiqueID != null || antiqueID != "")
        {
            AntiqueManager.Instance.EnableAntiqueDetail(antiqueID);
        }
    }
}