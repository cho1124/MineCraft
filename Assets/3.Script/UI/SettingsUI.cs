using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    private AudioManager audioManager;

    private void Start()
    {
        // AudioManager 인스턴스 할당
        audioManager = AudioManager.instance;

        // 슬라이더 값을 로드된 설정 값으로 초기화
        masterVolumeSlider.value = audioManager.masterVolume;
        musicVolumeSlider.value = audioManager.musicVolume;
        sfxVolumeSlider.value = audioManager.sfxVolume;

        // 슬라이더 값 변경 시 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioManager.SetMasterVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioManager.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioManager.SetSFXVolume(volume);
    }
}
