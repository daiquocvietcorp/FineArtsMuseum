using UnityEngine;
using UnityEngine.SceneManagement;

namespace Trigger
{
    public class ExitTrigger : MonoBehaviour
    {
        [field: SerializeField] private int exitCodeScene;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Load Scene Async
                SceneManager.LoadSceneAsync(exitCodeScene, LoadSceneMode.Single);
            }
        }
    }
}
