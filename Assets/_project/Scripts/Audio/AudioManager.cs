using UnityEngine;

namespace CharacterCreation
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private UiAudioClips _uiClips;

        public void PlayClick() => Play(_uiClips.click);
        public void PlayCancel() => Play(_uiClips.cancel);
        public void PlayError() => Play(_uiClips.error);
        public void PlayConfirm() => Play(_uiClips.confirm);

        public void PlayFadeIn() => Play(_uiClips.fadeIn);
        public void PlayFadeOut() => Play(_uiClips.fadeOut);

        private void Play(AudioClip clip)
        {
            if (clip == null) return;
            _audioSource.PlayOneShot(clip);
        }
    }
}

