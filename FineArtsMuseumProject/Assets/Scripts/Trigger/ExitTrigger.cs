using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Trigger
{
    public class ExitTrigger : MonoBehaviour
    {
        [field: SerializeField] private int exitId;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Load Scene Async
                ExitManager.Instance.ChangeScene(exitId);
            }
        }
    }
}
