using DesignPatterns;
using UnityEngine;

namespace Camera
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        [field: SerializeField] public UnityEngine.Camera mainCamera;
        [field: SerializeField] public UnityEngine.Camera xrCamera;
        [field: SerializeField] public CameraFollowPlayer cameraFollowPlayer;

        public void RegisterRotationDefault()
        {
            cameraFollowPlayer.RegisterRotationAction();
        }
    }
}
