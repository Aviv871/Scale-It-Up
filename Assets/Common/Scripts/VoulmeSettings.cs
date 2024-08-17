using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VoulmeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    private Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    private void Start() {
        if (PlayerPrefs.HasKey("MainVoulme")) {
            LoadVoulme();
        } else {
            SetVoulme();
        }
    }

    public void SetVoulme() {
        float volume = slider.value;
        mixer.SetFloat("MainVoulme", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("MainVoulme", volume);
    }

    private void LoadVoulme() {
        slider.value = PlayerPrefs.GetFloat("MainVoulme");
        SetVoulme();
    }
}
