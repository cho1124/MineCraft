using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonClickSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        Button button = GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound() {
        if (audioSource != null && audioSource.clip != null) {
            audioSource.Play();
        }
    }
}
