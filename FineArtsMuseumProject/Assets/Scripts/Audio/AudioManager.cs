using DesignPatterns;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [field: SerializeField] private AudioSource audioSource;
        [field: SerializeField] private AudioClip ambientClip;
        [field: SerializeField] private float volume;
        
        private bool _isMusicOn;
        
        private bool _isPlayingObject;
        private AudioClip _currentAudio;
        private float _currentVolume;

        public void FirstEnter()
        {
            _isMusicOn = true;
            _isPlayingObject = false;
            
            TurnMusic();
        }
        
        public void TurnMusic()
        {
            audioSource.clip = ambientClip;
            audioSource.volume = volume;
            audioSource.Play();
            audioSource.loop = true;
            
            switch (_isMusicOn)
            {
                case true when !_isPlayingObject:
                    audioSource.mute = true;
                    return;
                case false when !_isPlayingObject:
                    audioSource.mute = false;
                    break;
            }
        }
        
        public void EnterAudioArea(AudioClip audioClip, float audioVolume = 100)
        {
            _currentAudio = audioClip;
            _currentVolume = audioVolume;
            _isPlayingObject = false;
        }

        public void TurnAudio()
        {
            if (!_isPlayingObject)
            {
                PlayAudio();
                return;
            }
            
            StopAudio();
        }

        private void PlayAudio()
        {
            audioSource.clip = _currentAudio;
            audioSource.volume = _currentVolume;
            audioSource.mute = false;
            audioSource.Play();
            _isPlayingObject = true;
        }
        
        private void StopAudio()
        {
            audioSource.Stop();
            _isPlayingObject = false;
            TurnMusic();
        }
    }
}
