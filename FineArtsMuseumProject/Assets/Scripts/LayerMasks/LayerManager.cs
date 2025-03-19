using DesignPatterns;
using UnityEngine;

namespace LayerMasks
{
    public class LayerManager : MonoSingleton<LayerManager>
    {
        [field: SerializeField] public LayerMask groundLayer;
    }
}
