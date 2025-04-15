using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

namespace System
{
    public class TriggerManager : MonoSingleton<TriggerManager>
    {
        [field: SerializeField] private List<BoxCollider> TriggerColliders { get; set; }

        public void EnableTriggerColliders()
        {
            if (TriggerColliders == null || TriggerColliders.Count == 0) return;
            foreach (var triggerCollider in TriggerColliders)
            {
                triggerCollider.enabled = true;
            }
        }
        
        public void DisableTriggerColliders()
        {
            if (TriggerColliders == null || TriggerColliders.Count == 0) return;
            foreach (var triggerCollider in TriggerColliders)
            {
                triggerCollider.enabled = false;
            }
        }
    }
}
