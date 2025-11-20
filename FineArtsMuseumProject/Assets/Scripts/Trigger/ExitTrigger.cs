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
            if (PlatformManager.Instance.isSplitScene)
            {
                // Send Socket with ExitId
                Debug.Log("Send Socket with ExitId: " + exitId);
                return;
            }
            
            if (other.CompareTag("Player"))
            {
                // Load Scene Async
                ExitManager.Instance.ChangeScene(exitId);
            }
        }
    }
}
