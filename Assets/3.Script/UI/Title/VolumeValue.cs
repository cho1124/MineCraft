using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VolumeValue : MonoBehaviour
{
    // ========== Inspector public ==========
    /*
     각 스크롤바 인스펙터 창에 on value changed 에 함수가 제대로 연결이 안되어서 스크립트 수정하여 연결시도
     
     */

   // [SerializeField] private Scrollbar[] sr; //볼륨을 조절하는 스크롤바 배열.

    [SerializeField] private Scrollbar masterVolumeScrollbar;
    [SerializeField] private Scrollbar bgmVolumeScrollbar;
    [SerializeField] private Scrollbar sfxVolumeScrollbar;

    [SerializeField] private TextMeshProUGUI Full_volume_text = null; //전체 음량을 표시하는 UI 텍스트.
    [SerializeField] private TextMeshProUGUI background_volume_text = null; //배경 음량을 표시하는 UI 텍스트.
    [SerializeField] private TextMeshProUGUI effect_text = null; //효과 음량을 표시하는 UI 텍스트.

    private void Awake()
    {
        // for (int i = 0; i < sr.Length; i++)
        // {
        //     sr[i] = sr[i].GetComponent<Scrollbar>();
        //     int index = i; // 캡처된 인덱스 사용
        //     sr[i].onValueChanged.AddListener((value) => OnVolumeChanged(index, value));
        //     //스크롤바의 값이 변경될 때마다 OnVolumeChanged 메서드를 호출하여, 해당 스크롤바의 인덱스와 새로운 값을 전달
        // }

        // Master Volume Scrollbar 설정
        if (masterVolumeScrollbar != null) {
            masterVolumeScrollbar.onValueChanged.AddListener((value) => OnVolumeChanged(0, value));
        }

        // BGM Volume Scrollbar 설정
        if (bgmVolumeScrollbar != null) {
            bgmVolumeScrollbar.onValueChanged.AddListener((value) => OnVolumeChanged(1, value));
        }

        // SFX Volume Scrollbar 설정
        if (sfxVolumeScrollbar != null) {
            sfxVolumeScrollbar.onValueChanged.AddListener((value) => OnVolumeChanged(2, value));
        }

    }

    private void Start() {
        UpdateVolumeTexts(); //초기 볼륨 값을 UI 텍스트에 반영
    }

    public void OnVolumeChanged(int index, float value) { //스크롤바 값이 변경될 때 호출
        switch (index) {
            case 0:
                FullVolume(value);
                break;
            case 1:
                BackgroundVolume(value);
                break;
            case 2:
                EffectVolume(value);
                break;
        }
    }

    private void FullVolume(float value) // 전체 음량
    {
        Full_volume_text.text = $"전체 음량: {value * 100:F0}%";
        AudioManager.instance.SetMasterVolume(value);
    }

    private void BackgroundVolume(float value) // 배경 음량
    {
        background_volume_text.text = $"배경 음량: {value * 100:F0}%";
        AudioManager.instance.SetBGMVolume(value);
    }

    private void EffectVolume(float value) // 효과 음량
    {
        effect_text.text = $"효과 음량: {value * 100:F0}%";
        AudioManager.instance.SetSFXVolume(value);
    }

    private void UpdateVolumeTexts() { //각 스크롤바의 현재 값을 가져와 UI 텍스트를 초기화
        Full_volume_text.text = $"전체 음량: {masterVolumeScrollbar.value * 100:F0}%";
        background_volume_text.text = $"배경 음량: {bgmVolumeScrollbar.value * 100:F0}%";
        effect_text.text = $"효과 음량: {sfxVolumeScrollbar.value * 100:F0}%";
    }
}