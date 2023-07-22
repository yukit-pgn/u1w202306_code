using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Main
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] AudioMixer mixer;
        [SerializeField] string parameterName;
        
        void Awake()
        {
            // y = 20 * log10(x)
            // x = 10 ^ (y / 20)
            mixer.GetFloat(parameterName, out var volume);
            slider.value = Mathf.Pow(10f, volume / 20f);
            slider.onValueChanged.AddListener(ChangeVolume);
        }
        
        void ChangeVolume(float value)
        {
            var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
            mixer.SetFloat(parameterName, volume);
        }
    }
}
