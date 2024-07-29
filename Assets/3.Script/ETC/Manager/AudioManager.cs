using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


/*
BGM - 3개의 음악클립 랜덤으로 계속 재생하게 하려고 함

SFX- 음악클립 이름에 키워드 정확하게 적어두고 키워드 2개 찾기( <예시>"Creeper" "Die" )로 상황별 클립 불러오는 
방식으로 생각중   

[효과음이 많이 필요할 것 같아서 위의 방식과 같이 생각했고 상황별 클립 하나하나 정해서 필요시 불러와도 될것같음 ]


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

    // 키워드와 오디오 클립을 매핑할 딕셔너리
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
        bgmSource.loop = false; // 루프가 아니라 다음 클립을 재생하기 위해 설정

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
        // Resources/SFX 폴더에 있는 모든 오디오 클립을 로드합니다.
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");
        foreach (AudioClip clip in clips) {
            string clipName = clip.name.ToLower();

            // 클립 이름에 특정 키워드가 포함되어 있는지 확인합니다.
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
            // 필요한 경우 다른 키워드를 추가합니다.
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

//<예시>
//플레이어가 걷는 소리를 재생
//AudioManager.instance.PlayRandomSFX(AudioManager.instance.playerWalkClips);

//플레이어가 공격
//AudioManager.instance.PlayRandomSFX(AudioManager.instance.playerAttackClips);

//두 키워드를 기반으로 효과음 재생
//AudioManager.instance.PlayRandomSFX("walk", "attack");
