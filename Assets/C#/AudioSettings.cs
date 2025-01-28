using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        //initialize sliders with saved settings
        musicSlider.value = GameManager.settings.musicVolume * 10f;
        sfxSlider.value = GameManager.settings.audioVolume * 10f;

        UpdateVolumes();
    }

    private void Update()
    {
        //continuously update volume based on slider values
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    private void SetMusicVolume(float sliderValue)
    {
        //map slider (0-10) to decibels
        float normalizedVolume = sliderValue / 10f;
        float decibelVolume = MapToDecibel(normalizedVolume);

        //apply to AudioMixer and save normalized value
        audioMixer.SetFloat("MusicVolume", decibelVolume);
        GameManager.settings.musicVolume = normalizedVolume;
        SaveAndLoad.SaveSettings(GameManager.settings);
    }

    private void SetSFXVolume(float sliderValue)
    {
        float normalizedVolume = sliderValue / 10f;
        float decibelVolume = MapToDecibel(normalizedVolume);

        audioMixer.SetFloat("SFXVolume", decibelVolume);
        GameManager.settings.audioVolume = normalizedVolume; // Save normalized value
        SaveAndLoad.SaveSettings(GameManager.settings);
    }

    //converts linear volume to decibels
    private float MapToDecibel(float normalizedValue)
        => Mathf.Log10(normalizedValue) * 20f; 

    private void UpdateVolumes()
    {
        // Sync the sliders with saved settings and update mixer volumes
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }
}
