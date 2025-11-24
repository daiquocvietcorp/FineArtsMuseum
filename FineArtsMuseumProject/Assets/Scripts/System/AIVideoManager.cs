using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Video;

namespace System
{
    public class AIVideoManager : MonoSingleton<AIVideoManager>
    {
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        [field: SerializeField] private List<AIVideo> _aiVideos;
        [field: SerializeField] private Material videoMaterial;
        [field: SerializeField] private VideoPlayer videoPlayer;
        [field: SerializeField] private RenderTexture videoRenderTexture;
        
        private Dictionary<string, AIVideo> _aiVideoPlayers;
        
        private void Awake()
        {
            if (_aiVideos == null)
                return;

            _aiVideoPlayers = new Dictionary<string, AIVideo>();
            if(_aiVideoPlayers == null) return;
            
            foreach (var aiVideo in _aiVideos)
            {
                _aiVideoPlayers.Add(aiVideo.AntiqueObjectName, aiVideo);
            }
        }
        
        public bool IsObjectHasVideo(string objectName)
        {
            return _aiVideoPlayers.ContainsKey(objectName);
        }

        public void SetVideoMaterial(string objectName)
        {
            if(!_aiVideoPlayers.TryGetValue(objectName, out var aiVideo)) return;
            if(aiVideo.MeshRenderer == null || aiVideo.videoClip == null || aiVideo.originalMaterial == null) return;
            var materials = aiVideo.MeshRenderer.materials;
            
            var materialIndex = -1;

            for (var i = 0; i < materials.Length; i++)
            {
                if(materials[i].name != aiVideo.originalMaterial.name + " (Instance)")
                    continue;
                materialIndex = i;
                break;
            }
            
            if (materialIndex == -1) return;
            
            videoPlayer.clip = aiVideo.videoClip;
            videoPlayer.Prepare();
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
            
            materials[materialIndex] = videoMaterial;
            aiVideo.MeshRenderer.materials = materials;
            //materials[materialIndex].SetTexture(EmissionMap, videoRenderTexture);
        }

        public void StopVideoMaterial(string objectName)
        {
            if(!_aiVideoPlayers.TryGetValue(objectName, out var aiVideo)) return;
            if(aiVideo.MeshRenderer == null || aiVideo.videoClip == null || aiVideo.originalMaterial == null) return;
            var materials = aiVideo.MeshRenderer.materials;
            
            var materialIndex = -1;

            for (var i = 0; i < materials.Length; i++)
            {
                if(materials[i].name != videoMaterial.name + " (Instance)")
                    continue;
                materialIndex = i;
                break;
            }
            
            if (materialIndex == -1) return;
            
            materials[materialIndex] = aiVideo.originalMaterial;
            aiVideo.MeshRenderer.materials = materials;
        }
    }

    [Serializable]
    public class AIVideo
    {
        [field: SerializeField] public string AntiqueObjectName { get; set; }
        [field: SerializeField] public MeshRenderer MeshRenderer { get; set; }
        [field: SerializeField] public Material originalMaterial { get; set; }
        [field: SerializeField] public VideoClip videoClip { get; set; }
    }
}
