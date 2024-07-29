using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


/*
BGM - 3���� ����Ŭ�� �������� ��� ����ϰ� �Ϸ��� ��

SFX- ����Ŭ�� �̸��� Ű���� ��Ȯ�ϰ� ����ΰ� Ű���� 2�� ã��( <����>"Creeper" "Die" )�� ��Ȳ�� Ŭ�� �ҷ����� 
������� ������   

[ȿ������ ���� �ʿ��� �� ���Ƽ� ���� ��İ� ���� �����߰� ��Ȳ�� Ŭ�� �ϳ��ϳ� ���ؼ� �ʿ�� �ҷ��͵� �ɰͰ��� ]


*/
public class AudioManager : MonoBehaviour {
    public static AudioManager instance = null;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float bgmVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    public AudioMixer audioMixer;

    [Header("UI Sliders")]
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("BGM")]
    public AudioClip[] bgmClips;
    private AudioSource bgmSource;

    private AudioSource sfxSource;

    // Ű����� ����� Ŭ���� ������ ��ųʸ�
    private Dictionary<string, List<AudioClip>> sfxClips;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        LoadSettings();

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;

        if (bgmVolumeSlider != null)
            bgmVolumeSlider.value = bgmVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = false; // ������ �ƴ϶� ���� Ŭ���� ����ϱ� ���� ����

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;

        sfxClips = new Dictionary<string, List<AudioClip>>();

        LoadSFXClips();

        if (bgmClips.Length > 0) {
            StartCoroutine(PlayRandomBGM());
        }
    }

    public void SetMasterVolume(float volume) {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    public void SetBGMVolume(float volume) {
        bgmVolume = volume;
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVolume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
    }

    public void SetSFXVolume(float volume) {
        sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadSettings() {
        if (PlayerPrefs.HasKey("MasterVolume")) {
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        }

        if (PlayerPrefs.HasKey("BGMVolume")) {
            SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume"));
        }

        if (PlayerPrefs.HasKey("SFXVolume")) {
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        }
    }

    IEnumerator PlayRandomBGM() {
        while (true) {
            AudioClip clip = bgmClips[Random.Range(0, bgmClips.Length)];
            bgmSource.clip = clip;
            bgmSource.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }

    public void PlaySFX(AudioClip clip) {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomSFX(string keyword) {
        if (sfxClips.ContainsKey(keyword) && sfxClips[keyword].Count > 0) {
            AudioClip clip = sfxClips[keyword][Random.Range(0, sfxClips[keyword].Count)];
            PlaySFX(clip);
        }
    }

    private void LoadSFXClips() {
        // Resources/SFX ������ �ִ� ��� ����� Ŭ���� �ε��մϴ�.
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");
        foreach (AudioClip clip in clips) {
            string clipName = clip.name.ToLower();

            // Ŭ�� �̸��� Ư�� Ű���尡 ���ԵǾ� �ִ��� Ȯ���մϴ�.
            if (clipName.Contains("walk")) {
                AddSFXClip("walk", clip);
            }
            if (clipName.Contains("run")) {
                AddSFXClip("run", clip);
            }
            if (clipName.Contains("attack")) {
                AddSFXClip("attack", clip);
            }
            if (clipName.Contains("animaldeath")) {
                AddSFXClip("animaldeath", clip);
            }
            if (clipName.Contains("monsterdeath")) {
                AddSFXClip("monsterdeath", clip);
            }
            // �ʿ��� ��� �ٸ� Ű���带 �߰��մϴ�.
        }

   //     private void AddSFXClip(string keyword, AudioClip clip) {
   //         if (!sfxClips.ContainsKey(keyword)) {
   //             sfxClips[keyword] = new List<AudioClip>();
   //         }
   //         sfxClips[keyword].Add(clip);
   //     }
   //
   //     public void PlayRandomSFX(string keyword1, string keyword2) {
   //         List<AudioClip> combinedClips = new List<AudioClip>();
   //
   //         if (sfxClips.ContainsKey(keyword1)) {
   //             combinedClips.AddRange(sfxClips[keyword1]);
   //         }
   //
   //         if (sfxClips.ContainsKey(keyword2)) {
   //             combinedClips.AddRange(sfxClips[keyword2]);
   //         }
   //
   //         if (combinedClips.Count > 0) {
   //             AudioClip clip = combinedClips[Random.Range(0, combinedClips.Count)];
   //             PlaySFX(clip);
   //         }
   //     }
   // }
}

//<����>
//�÷��̾ �ȴ� �Ҹ��� ���
//AudioManager.instance.PlayRandomSFX(AudioManager.instance.playerWalkClips);

//�÷��̾ ����
//AudioManager.instance.PlayRandomSFX(AudioManager.instance.playerAttackClips);

//�� Ű���带 ������� ȿ���� ���
//AudioManager.instance.PlayRandomSFX("walk", "attack");
