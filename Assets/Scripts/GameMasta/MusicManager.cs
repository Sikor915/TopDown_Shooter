using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] AudioSource audioSource;
    [Header("Music Clips")]
    [SerializeField] AudioClip shopMusic;
    [SerializeField] List<AudioClip> battleMusic;
    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 1f;

    Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        audioSource.loop = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
        if (scene.name == "Shop" || scene.name == "Main Menu")
        {
            PlayMusic(shopMusic);
        }
        else
        {
            StartBattleMusic();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeToNewClip(clip));
    }

    public void StopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeToNewClip(null));
    }
    void StartBattleMusic()
    {
        PlayRandomBattleMusic();
    }

    void PlayRandomBattleMusic()
    {
        if (battleMusic == null || battleMusic.Count == 0)
            return;

        AudioClip newClip = battleMusic[Random.Range(0, battleMusic.Count)];
        StartCoroutine(FadeToNewClip(newClip));
    }

    IEnumerator FadeToNewClip(AudioClip newClip)
    {
        // fade out
        if (audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
        }

        // switch music
        audioSource.clip = newClip;
        audioSource.Play();

        // fade in
        float targetVolume = 1f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }

        // now wait for clip to finish and then play next
        StartCoroutine(WaitAndPlayNext());
    }

    IEnumerator WaitAndPlayNext()
    {
        if (audioSource.clip == null)
            yield break;

        float endTime = Time.time + audioSource.clip.length;

        while (Time.time < endTime)
        {
            yield return null;
        }

        // after this track ends, pick a new battle music
        PlayRandomBattleMusic();
    }
}

