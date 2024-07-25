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
        // AudioManager �ν��Ͻ� �Ҵ�
        audioManager = AudioManager.instance;

        // �����̴� ���� �ε�� ���� ������ �ʱ�ȭ
        masterVolumeSlider.value = audioManager.masterVolume;
        bgmVolumeSlider.value = audioManager.bgmVolume;
        sfxVolumeSlider.value = audioManager.sfxVolume;

        // �����̴� �� ���� �� �̺�Ʈ ���
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
