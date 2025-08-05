using UnityEngine;

namespace CharacterCreation
{
    [CreateAssetMenu(menuName = "Audio/AudioClips")]
    public class UiAudioClips : ScriptableObject
    {
        public AudioClip click;
        public AudioClip cancel;
        public AudioClip error;
        public AudioClip confirm;
        public AudioClip fadeIn;
        public AudioClip fadeOut;
    }
}

