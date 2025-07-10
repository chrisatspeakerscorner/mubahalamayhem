using DG.Tweening;
using EasyTransition;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quranButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private Button _quranVerseBackButton;
    [SerializeField] private Button _tutorialBackButton;

    [SerializeField] private SettingsController _settingsController;

    [SerializeField] private GameObject _quranVerseOfTheDay;
    [SerializeField] private GameObject _tutorial;

    [SerializeField] private RectTransform _logoTransform;

    [SerializeField] private ParticleSystem _ps;
    [SerializeField] private CanvasGroup _gridCanvasGroup;

    [SerializeField] private Canvas _characterSelectionCanvas;
    [SerializeField] private Canvas _introCanvas;
    [SerializeField] private Canvas _gameCanvas;

    [SerializeField] private AudioSource _as;

    [SerializeField] private AudioClip _transitionSwipe;

    [SerializeField] private BackgroundMusicController _backgroundMusic;

    [SerializeField] private AudioClip _thunderSFX;
    [SerializeField] private AudioClip _mubahalaSFX;
    [SerializeField] private AudioClip _tutorialSFX;

    [SerializeField] private TransitionSettings _transitionSettings;

    private TransitionManager _transitionManager;

    private Tween _makeLightning;
    private Tween _showButtons;
    private Tween _playMusic;
    private Tween _animateLogo;
    private Tween _fadeInBackground;

    private void OnEnable()
    {
        _backgroundMusic.StopMusic();

        _fadeInBackground.Kill();

        _gridCanvasGroup.alpha = 0F;

        _quranVerseOfTheDay.SetActive(false);
        _tutorial.SetActive(false);

        _playButton.gameObject.SetActive(false);
        _quranButton.gameObject.SetActive(false);
        _tutorialButton.gameObject.SetActive(false);

        _settingsController.SetButtonState(false);

        _playButton.onClick.AddListener(HandlePlayButtonPressed);
        _quranButton.onClick.AddListener(HandleQuranButtonPressed);
        _tutorialButton.onClick.AddListener(HandleTutorialButtonPressed);

        _quranVerseBackButton.onClick.AddListener(HandleQuranVerseBackButtonPressed);
        _tutorialBackButton.onClick.AddListener(HandleTutorialBackButtonPressed);

        _as.PlayOneShot(_mubahalaSFX);

        _logoTransform.localScale = Vector3.zero; // Start hidden

        DOTween.Kill(_makeLightning);
        DOTween.Kill(_showButtons);

        _makeLightning = DOVirtual.DelayedCall(2.5F, MakeLightning, false).SetLink(gameObject);
        _showButtons = DOVirtual.DelayedCall(5F, ShowButton, false).SetLink(gameObject);

        _introCanvas.gameObject.SetActive(false);
        _gameCanvas.gameObject.SetActive(false);

        // Animate scale to 1 with a pop effect
        DOTween.Kill(_animateLogo);
        _animateLogo = _logoTransform.DOScale(Vector3.one, 1F)
                         .SetEase(Ease.OutBack)
                         .SetLink(gameObject)
                         .SetDelay(3F).OnComplete(BringInBackground);
    }


    private void HandleTutorialButtonPressed()
    {
        _tutorial.SetActive(true);

        _as.Stop();

        _as.PlayOneShot(_tutorialSFX);
    }

    private void HandleTutorialBackButtonPressed()
    {
        _tutorial.SetActive(false);
    }

    private void Start()
    {
        _transitionManager = TransitionManager.Instance();
    }

    private void HandleQuranVerseBackButtonPressed()
    {
        _quranVerseOfTheDay.gameObject.SetActive(false);
    }

    private void HandleQuranButtonPressed()
    {
        _quranVerseOfTheDay.gameObject.SetActive(true);
    }

    private void ShowButton()
    {
        DOTween.Kill(_playMusic);

        _playMusic = DOVirtual.DelayedCall(.1F, PlayMusic, false).SetLink(gameObject);

        _playButton.gameObject.SetActive(true);
        _quranButton.gameObject.SetActive(true);
        _tutorialButton.gameObject.SetActive(true);
        _settingsController.SetButtonState(true);
    }

    private void PlayMusic()
    {
        _backgroundMusic.PlayMusicRandom();
    }

    private void BringInBackground()
    {
        DOTween.Kill(_fadeInBackground);
        _fadeInBackground = _gridCanvasGroup.DOFade(1F, 3F).SetLink(gameObject);
    }

    private void MakeLightning()
    {
        _ps.Play();
        _as.PlayOneShot(_thunderSFX);
    }

    private void HandlePlayButtonPressed()
    {
        _transitionManager.onTransitionCutPointReached -= OpenCharacterSelectionScreen;
        _transitionManager.onTransitionCutPointReached += OpenCharacterSelectionScreen;

        _as.PlayOneShot(_transitionSwipe);

        _transitionManager.Transition(_transitionSettings, 0F);
    }

    private void OpenCharacterSelectionScreen()
    {
        _characterSelectionCanvas.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }

}
