using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Toggle fullscreenToggle;

    [SerializeField] Image brightnessOverlay;
    [SerializeField] AudioMixer audioMixer;

    float _musicVolume = 1f;
    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(_musicVolume) * 20);
        }
    }
    float _sfxVolume = 1f;
    public float SFXVolume
    {
        get { return _sfxVolume; }
        set
        {
            _sfxVolume = value;
            PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(_sfxVolume) * 20);
        }
    }
    bool _isFullscreen = true;
    public bool IsFullscreen
    {
        get { return _isFullscreen; }
        set
        {
            _isFullscreen = value;
            PlayerPrefs.SetInt("IsFullscreen", _isFullscreen ? 1 : 0);
            Screen.fullScreen = _isFullscreen;
        }
    }
    float _brightness = 1f;
    public float Brightness
    {
        get { return _brightness; }
        set
        {
            _brightness = value;
            PlayerPrefs.SetFloat("Brightness", _brightness);
            Color overlayColor = brightnessOverlay.color;
            overlayColor.a = 1f - _brightness;
            brightnessOverlay.color = overlayColor;
        }
    }


    void Awake()
    {
        // Ensure only one instance of Settings exists - I AM TIRED OF THIS SHIT
        var settingsList = FindObjectsByType<Settings>(FindObjectsSortMode.None);
        if (settingsList.Length > 1 && settingsList[0] != this)
        {
            Brightness = settingsList[1].Brightness;
            MusicVolume = settingsList[1].MusicVolume;
            SFXVolume = settingsList[1].SFXVolume;
            IsFullscreen = settingsList[1].IsFullscreen;
            Destroy(settingsList[1].gameObject);
        }
        else if (settingsList.Length > 1 && settingsList[1] != this)
        {
            Brightness = settingsList[0].Brightness;
            MusicVolume = settingsList[0].MusicVolume;
            SFXVolume = settingsList[0].SFXVolume;
            IsFullscreen = settingsList[0].IsFullscreen;
            Destroy(settingsList[0].gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            MusicVolume = 1f;
        }
        musicSlider.value = MusicVolume;
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            SFXVolume = 1f;
        }
        sfxSlider.value = SFXVolume;
        if (PlayerPrefs.HasKey("IsFullscreen"))
        {
            IsFullscreen = PlayerPrefs.GetInt("IsFullscreen") == 1;
        }
        else
        {
            IsFullscreen = true;
        }
        fullscreenToggle.isOn = IsFullscreen;
        if (PlayerPrefs.HasKey("Brightness"))
        {
            Brightness = PlayerPrefs.GetFloat("Brightness");
        }
        else
        {
            Brightness = 1f;
        }
        brightnessSlider.value = Brightness;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBrightnessChanged()
    {
        Brightness = brightnessSlider.value;
    }

    public void OnMusicVolumeChanged()
    {
        MusicVolume = musicSlider.value;
    }

    public void OnSFXVolumeChanged()
    {
        SFXVolume = sfxSlider.value;
    }
}
