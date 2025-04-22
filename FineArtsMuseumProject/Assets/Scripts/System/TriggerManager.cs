using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace System
{
    public class TriggerManager : MonoSingleton<TriggerManager>
    {
        [field: SerializeField] private List<BoxCollider> TriggerColliders { get; set; }
        [field: SerializeField] private List<BoxCollider> AllTriggerColliders { get; set; }

        private void Start()
        {
            DisableTriggerColliders();
        }

        public void EnableTriggerColliders()
        {
            if (TriggerColliders == null || TriggerColliders.Count == 0) return;
            foreach (var triggerCollider in TriggerColliders)
            {
                triggerCollider.enabled = true;
                //triggerCollider.gameObject.SetActive(true);
            }
        }
        
        public void DisableTriggerColliders()
        {
            if (TriggerColliders == null || TriggerColliders.Count == 0) return;
            foreach (var triggerCollider in TriggerColliders)
            {
                triggerCollider.enabled = false;
                //triggerCollider.gameObject.SetActive(false);
            }
        }
        
        public void EnableAllTriggerColliders()
        {
            if (AllTriggerColliders == null || AllTriggerColliders.Count == 0) return;
            foreach (var triggerCollider in AllTriggerColliders)
            {
                triggerCollider.enabled = true;
                //triggerCollider.gameObject.SetActive(true);
            }
        }
        
        public void DisableAllTriggerColliders()
        {
            if (AllTriggerColliders == null || AllTriggerColliders.Count == 0) return;
            foreach (var triggerCollider in AllTriggerColliders)
            {
                triggerCollider.enabled = false;
                //triggerCollider.gameObject.SetActive(false);
            }
        }
    }
}
