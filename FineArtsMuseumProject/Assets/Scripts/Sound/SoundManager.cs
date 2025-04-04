using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [field: SerializeField] private Transform onSound;
        [field: SerializeField] private Transform offSound;
        private Button _button;
        private bool _isPlaying = true;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _isPlaying = !_isPlaying;
            if (_isPlaying)
            {
                onSound.gameObject.SetActive(true);
                offSound.gameObject.SetActive(false);
                // Play sound
            }
            else
            {
                onSound.gameObject.SetActive(false);
                offSound.gameObject.SetActive(true);
                // Stop sound
            }
        }
    }
}
