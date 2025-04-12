using System.Collections.Generic;
using UnityEngine;

namespace System
{
    [CreateAssetMenu(fileName = "SliderScriptableObject", menuName = "ScriptableObjects/SliderScriptableObject")]
    public class SliderScriptableObject : ScriptableObject
    {
        [SerializeField] public List<SliderData> TopSliderSprites;
        [SerializeField] public List<SliderData> BottomSliderSprites;
        [SerializeField] public Sprite topScreenSprite;
        [SerializeField] public Sprite bottomScreenSprite;
    }
    
    [Serializable]
    public class SliderData
    {
        [SerializeField] public Sprite SliderSprite;
        [SerializeField] public string Title;
        [SerializeField] public string Subtitle;
        [SerializeField] public string TitleEnglish;
        [SerializeField] public string SubtitleEnglish;
    }
}
