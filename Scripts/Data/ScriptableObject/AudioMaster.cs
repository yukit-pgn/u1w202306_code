using UnityEngine;
using UnityEngine.Audio;

namespace Main.Data
{
    [CreateAssetMenu(menuName = "Master/Audio")]
    public class AudioMaster : ScriptableObject
    {
        public AudioMixerGroup bgmAudioMixerGroup;
		public AudioMixerGroup seAudioMixerGroup;
    }
}
