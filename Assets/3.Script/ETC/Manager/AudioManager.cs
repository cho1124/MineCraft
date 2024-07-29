using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


/*
BGM - 3���� ����Ŭ�� �������� ��� ����ϰ� �Ϸ��� ��

SFX- ����Ŭ�� �̸��� Ű���� ��Ȯ�ϰ� ����ΰ� Ű���� 2�� ã��( <����>"Creeper" "Die" )�� ��Ȳ�� Ŭ�� �ҷ����� 
������� ������   
ȿ������ ��ųʸ��� �����ϰ� Ű���带 ����Ͽ� Ű���忡 �´� ȿ���� ������ ����ϴ� ���


[ȿ������ ���� �ʿ��� �� ���Ƽ� ���� ��İ� ���� �����߰� ��Ȳ�� Ŭ�� �ϳ��ϳ� ���ؼ� �ʿ�� �ҷ��͵� �ɰͰ��� ]

1. ��Ȳ�� ����� Ŭ���� ���� ������ 
   PlaySFX(AudioClip clip)
   <����> 
   AudioManager.instance.PlaySFX(jumpSound);

2. Ű���忡 �´� ����� Ŭ���� ã�ƿ� ����� ��
   PlayRandomSFX(string keyword1, string keyword2)
   <����>
   AudioManager.instance.PlayRandomSFX("walk", "attack");

��Ű����� <����� - �ൿ> �̷��� �Ǿ������� ù���ڴ� �빮��
���� ����ڿ��� Humanoid- / Enderman- / Chicken- / Creeper- / Object- ���� ���� 
�ൿ���� Attack / Die / Arrow / Click / Landing / Get ���� ����

��PlayerPrefs : ����Ƽ���� �����ϴ� ���� Ŭ����:�����ѵ����͸� ���������� �����ϰ� �ҷ����µ� ���

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
            return;
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
            StartCoroutine(PlayRandomBGM()); //BGM3������ �����ϰ� ����ϴ� �ڷ�ƾ ����
        }
    }

    public void SetMasterVolume(float volume) { //�����ͺ����� �����ϰ� PlayerPrefs�� ����
        masterVolume = volume;
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    public void SetBGMVolume(float volume) { //������� ������ ����
        bgmVolume = volume;
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVolume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
    }

    public void SetSFXVolume(float volume) { //ȿ���� ������ ����
        sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadSettings() { //PlayerPrefs�� ����� ���� ������ �ҷ���
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

    private IEnumerator PlayRandomBGM() { //������� Ŭ�� �� �ϳ��� �����ϰ� �����Ͽ� ���
        while (true) {
            AudioClip clip = bgmClips[Random.Range(0, bgmClips.Length)];
            bgmSource.clip = clip;
            bgmSource.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }

    public void PlaySFX(AudioClip clip) { //�־��� ����� Ŭ���� ���
        sfxSource.PlayOneShot(clip);
    }

    // �� ���� Ű���带 ��� �����ϴ� ȿ���� Ŭ�� �� �ϳ��� �����ϰ� �����Ͽ� ���
    public void PlayRandomSFX(string keyword1, string keyword2) {
        List<AudioClip> combinedClips = new List<AudioClip>();

        //��ųʸ��� �� Ű���带 �ݺ��ϸ鼭, Ű���忡 keyword1 �Ǵ� keyword2�� ���Ե� ��� �ش� Ŭ������ combinedClips ����Ʈ�� �߰�
        foreach (var kvp in sfxClips) {
            if (kvp.Key.Contains(keyword1) || kvp.Key.Contains(keyword2)) {
                combinedClips.AddRange(kvp.Value);
            }
        }

        if (combinedClips.Count > 0) {
            AudioClip clip = combinedClips[Random.Range(0, combinedClips.Count)];
            PlaySFX(clip);
        }
    }

    private void LoadSFXClips() {
        //SFX ������ �ִ� ��� ����� Ŭ���� �ε��մϴ�.
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");
        foreach (AudioClip clip in clips) {
            // Ŭ�� �̸��� '-'�� �и��Ͽ� �� �κ��� Ű����� �����մϴ�.
            string[] keywords = clip.name.Split('-');
            if (keywords.Length == 2) {
                foreach (var keyword in keywords) {
                    AddSFXClip(keyword, clip);
                }
            }
        }

        void AddSFXClip(string keyword, AudioClip clip) { //Ư�� Ű���忡 �ش��ϴ� ����� Ŭ���� ��ųʸ��� �߰�
            if (!sfxClips.ContainsKey(keyword)) {
                sfxClips[keyword] = new List<AudioClip>();
            }
            sfxClips[keyword].Add(clip);
        }
    }
}
