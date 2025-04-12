using System.Collections;
using System.Collections.Generic;
using Camera;
using DesignPatterns;
using Player;
using Trigger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System
{
    public class ExitManager : MonoSingleton<ExitManager>
    {
        [SerializeField] private ExitData exitData;
        private Dictionary<int, ExitDataObject> _exitDataDictionary;
        private int _currentSceneId;
        private Coroutine _loadSceneCoroutine;

        private void Start()
        {
            _exitDataDictionary = new Dictionary<int, ExitDataObject>();
            foreach (var exitDataObject in exitData.exitDataObjects)
            {
                _exitDataDictionary.Add(exitDataObject.id, exitDataObject);
            }
            _currentSceneId = SceneManager.GetActiveScene().buildIndex;
        }

        public void ChangeScene(int id)
        {
            if(!_exitDataDictionary.TryGetValue(id, out var exitDataObject)) return;
            if(exitDataObject.toSceneId == _currentSceneId) return;
            
            if(SceneLog.IsFirstScene)
                SceneLog.IsFirstScene = false;
            
            SceneLog.PreviousSceneId = _currentSceneId;
            
            if(_loadSceneCoroutine != null)
            {
                StopCoroutine(_loadSceneCoroutine);
            }
            _loadSceneCoroutine = StartCoroutine(LoadScene(exitDataObject));
        }

        private IEnumerator LoadScene(ExitDataObject exitDataObject)
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync(exitDataObject.toSceneId);
            
            while (loadSceneAsync is { isDone: false })
            {
                // Bạn có thể hiển thị thanh tiến trình tại đây, ví dụ:
                // Debug.Log(asyncLoad.progress);
                yield return null;
            }
            
            //CharacterManager.Instance.SetCharacterInfo(exitDataObject.playerPosition, exitDataObject.playerRotation);
            //CameraManager.Instance.cameraFollowPlayer.SetCamera(exitDataObject.cameraPosition, exitDataObject.cameraRotation);
        }
    }
}
