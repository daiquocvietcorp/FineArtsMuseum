using DesignPatterns;
using UnityEngine;

namespace Camera
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        [field: SerializeField] public UnityEngine.Camera mainCamera;
        [field: SerializeField] public UnityEngine.Camera xrCamera;
    }
}
