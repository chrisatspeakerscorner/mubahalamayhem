using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Toggle _soundButton;

    [SerializeField] private GameObject _settings;

    [SerializeField] private AudioSource _gameAS;
    [SerializeField] private AudioSource _character1AS;
    [SerializeField] private AudioSource _character2AS;
    [SerializeField] private AudioSource _characterSelectionAS;
    [SerializeField] private AudioSource _menuAS;
    [SerializeField] private AudioSource _cardAS;
    [SerializeField] private AudioSource _allahAS;
    [SerializeField] private AudioSource _sceneSelectionAS;
    [SerializeField] private AudioSource _settingsAS;

    [SerializeField] private AudioSource _musicAS;

    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private string _musicKey = "music_volume";
    private string _sfxKey = "sfx_volume";

    private void Awake()
    {
        _musicSlider.onValueChanged.AddListener(HandleMusicVolumeChange);
        _sfxSlider.onValueChanged.AddListener(HandleSFXVolumeChange);

        _settingsButton.onClick.AddListener(HandleSettingsButtonPressed);
        _backButton.onClick.AddListener(HandleBackButtonPressed);

        LoadVolumePrefs();
    }

    private void LoadVolumePrefs()
    {
        if (!PlayerPrefs.HasKey(_sfxKey))
            return;

        if (!PlayerPrefs.HasKey(_musicKey))
            return;

        float sfxVolume = PlayerPrefs.GetFloat(_sfxKey);
        float musicVolume = PlayerPrefs.GetFloat(_musicKey);

        _gameAS.volume = sfxVolume;
        _character1AS.volume = sfxVolume;
        _character2AS.volume = sfxVolume;
        _characterSelectionAS.volume = sfxVolume;
        _allahAS.volume = sfxVolume;
        _menuAS.volume = sfxVolume;
        _cardAS.volume = sfxVolume;
        _sceneSelectionAS.volume = sfxVolume;
        _settingsAS.volume = sfxVolume;

        _musicAS.volume = musicVolume;

        _musicSlider.value = musicVolume;
        _sfxSlider.value = sfxVolume;
    }

    private void HandleSFXVolumeChange(float volume)
    {
        PlayerPrefs.SetFloat(_sfxKey, volume);

        PlayerPrefs.Save();

        _gameAS.volume = volume;
        _character1AS.volume = volume;
        _character2AS.volume = volume;
        _characterSelectionAS.volume = volume;
        _allahAS.volume = volume;
        _menuAS.volume = volume;
        _cardAS.volume = volume;
        _sceneSelectionAS.volume = volume;
        _settingsAS.volume = volume;
    }

    private void HandleMusicVolumeChange(float volume)
    {
        PlayerPrefs.SetFloat(_musicKey, volume);

        PlayerPrefs.Save();

        _musicAS.volume = volume;
    }

    public void SetButtonState(bool state)
    {
        transform.gameObject.SetActive(state);

        _settingsButton.gameObject.SetActive(state);
    }

    private void HandleBackButtonPressed()
    {
        _settings.SetActive(false);
    }

    private void HandleSettingsButtonPressed()
    {
        _settings.SetActive(true);
    }
}
