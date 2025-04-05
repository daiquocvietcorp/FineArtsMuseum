using System;
using Audio;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Sound
{
    public class SoundManager : UIBasic
    {
        [field: SerializeField] private Transform onSound;
        [field: SerializeField] private Transform offSound;
        private Button _button;
        private bool _isPlaying = false;

        public override void ActionUI(Action action = null)
        {
            base.ActionUI(action);
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
            OnClick();
        }

        private void OnClick()
        {
            _isPlaying = !_isPlaying;
            if (_isPlaying)
            {
                onSound.gameObject.SetActive(true);
                offSound.gameObject.SetActive(false);
                //AudioSubtitleManager.Instance.
                //AudioManager.Instance
                // Play sound
                
                AudioSubtitleManager.Instance.TurnAmbientSound();
            }
            else
            {
                onSound.gameObject.SetActive(false);
                offSound.gameObject.SetActive(true);
                // Stop sound
                
                AudioSubtitleManager.Instance.TurnAmbientSound();
            }
        }
    }
}
