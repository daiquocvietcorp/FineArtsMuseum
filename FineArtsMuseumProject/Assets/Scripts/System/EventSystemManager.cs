using UnityEngine;

namespace System
{
    public class EventSystemManager : MonoBehaviour
    {
        [field: SerializeField] private Transform standAloneEventSystem;
        [field: SerializeField] private Transform cloudEventSystem;
        
        private void Awake()
        {
            if (PlatformManager.Instance.IsStandalone)
            {
                standAloneEventSystem.gameObject.SetActive(true);
                cloudEventSystem.gameObject.SetActive(false);
            }
            else if (PlatformManager.Instance.IsCloud)
            {
                standAloneEventSystem.gameObject.SetActive(false);
                cloudEventSystem.gameObject.SetActive(true);
            }
        }
    }
}
