using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VolumeValue : MonoBehaviour
{
    // ========== Inspector public ==========

    [SerializeField] private Scrollbar[] sr;

    [SerializeField] private TextMeshProUGUI Full_volume_text = null;
    [SerializeField] private TextMeshProUGUI background_volume_text = null;
    [SerializeField] private TextMeshProUGUI effect_text = null;

    private void Awake()
    {
        for (int i = 0; i < sr.Length; i++)
        {
            sr[i] = sr[i].GetComponent<Scrollbar>();
        }
    }

    private void Start()
    {
        Full_volume_text.text = $"전체 음량: {sr[0].value * 100:F0}%";
        background_volume_text.text = $"배경 음량: {sr[1].value * 100:F0}%";
        effect_text.text = $"효과 음량: {sr[2].value * 100:F0}%";
    }

    private void Update()
    {
        FullVolume();
        BackgroundVolume();
        EffectVolume();
    }

    private void FullVolume() // 전체 음량
    {
        Full_volume_text.text = $"전체 음량: {sr[0].value * 100:F0}%";
        AudioManager.instance.SetMasterVolume(sr[0].value);
    }

    private void BackgroundVolume() // 배경 음량
    {
        background_volume_text.text = $"배경 음량: {sr[1].value * 100:F0}%";
        AudioManager.instance.SetBGMVolume(sr[1].value);
    }

    private void EffectVolume() // 효과 음량
    {
        effect_text.text = $"효과 음량: {sr[2].value * 100:F0}%";
        AudioManager.instance.SetSFXVolume(sr[2].value);
    }
}