using System.Collections.Generic;
using Camera;
using DesignPatterns;
using Player;
using UnityEngine;

namespace Utils
{
    public class ScriptableObjectManager : MonoSingleton<ScriptableObjectManager>
    {
        [field: SerializeField] private BuildingCollectSO buildingCollectSo;
        private Dictionary<int, CollectPlayer> _collectPlayerDictionary;
        private Dictionary<int, CollectCamera> _collectCameraDictionary;
        
        public void Initialize()
        {
            InitializeBuildingCollectSo();
        }
        
        private void InitializeBuildingCollectSo()
        {
            if(buildingCollectSo == null || buildingCollectSo.floors == null || buildingCollectSo.floors.Count == 0) return;
            _collectPlayerDictionary = new Dictionary<int, CollectPlayer>();
            _collectCameraDictionary = new Dictionary<int, CollectCamera>();
            foreach (var collectFloor in buildingCollectSo.floors)
            {
                if(_collectPlayerDictionary.ContainsKey(collectFloor.floor)) continue;
                var collectPlayer = collectFloor.player;
                if(collectPlayer != null)
                    _collectPlayerDictionary.Add(collectFloor.floor, collectPlayer);
                
                var collectCamera = collectFloor.camera;
                if(collectCamera != null)
                    _collectCameraDictionary.Add(collectFloor.floor, collectCamera);
            }
        }
        
        public CollectPlayer GetCollectPlayer(int floor)
        {
            if(_collectPlayerDictionary == null || !_collectPlayerDictionary.TryGetValue(floor, out var collectPlayer)) return null;
            return collectPlayer;
        }
        
        public CollectCamera GetCollectCamera(int floor)
        {
            if(_collectCameraDictionary == null || !_collectCameraDictionary.TryGetValue(floor, out var collectCamera)) return null;
            return collectCamera;
        }

        public void Test(int floor)
        {
            CharacterManager.Instance.SetFloorForCharacter(floor);
            CameraManager.Instance.SetCameraRotationByFloor(floor);
        }
    }
}
