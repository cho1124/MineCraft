using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    private AudioManager audioManager;

    private void Start()
    {
        // AudioManager 인스턴스 할당
        audioManager = AudioManager.instance;

        // 슬라이더 값을 로드된 설정 값으로 초기화
        masterVolumeSlider.value = audioManager.masterVolume;
        bgmVolumeSlider.value = audioManager.bgmVolume;
        sfxVolumeSlider.value = audioManager.sfxVolume;

        // 슬라이더 값 변경 시 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        audioManager.SetMasterVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioManager.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioManager.SetSFXVolume(volume);
    }
}
