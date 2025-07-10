using DG.Tweening;
using Mubahala.View;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;
using TextAsset = UnityEngine.TextAsset;

namespace Mubahala.Controller
{
    public class OptionData
    {
        [JsonProperty("text")]
        public string Text;

        [JsonProperty("optionType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OptionType Type;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OptionType
    {
        One = 1, Two = 2, Three = 3
    }

    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject _optionPrefab;

        [SerializeField] private Transform _optionContainer;

        [SerializeField] private Transform _optionSpawnContainer;

        private List<GameObject> _options = new();

        private GameObject[] _optionsChosen = new GameObject[3];

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _endButton;

        [SerializeField] private Allah _allah;
        
        [SerializeField] private Character _character1;
        [SerializeField] private Character _character2;

        [SerializeField] private Image _stage;

        [SerializeField] private Animator _endgameOverlayAnimator;
        [SerializeField] private Animator _roundBackgroundOverlayAnimator;

        [SerializeField] private TextAnimator _speachText1;
        [SerializeField] private TextAnimator _speachText2;

        [SerializeField] private CanvasGroup _endgameCanvasGroup;
        [SerializeField] private CanvasGroup _creditsCanvasGroup;

        [SerializeField] private Canvas _menuCanvasGroup;
        [SerializeField] private Canvas _gameCanvasGroup;

        [SerializeField] private CardController _cardController;

        [SerializeField] private AudioSource _as;
        [SerializeField] private BackgroundMusicController _backgroundMusicController;
        [SerializeField] private AudioClip  _cardsDeltSFX;
        [SerializeField] private AudioClip  _playButtonPressedSFX;
        [SerializeField] private AudioClip  _thunderSFX;

        private Tween _currentDelayTween;
        private Tween _playAllahSoundTween;
        private Tween _waitBeforeHidingAllahTween;

        private List<OptionData> _optionData;

        private Animator _allahAnimator;

        private bool _gameOver = false;
        private bool _skippedShowingAllOptions = false;

        private Tween _endgameCanvasGroupTween;

        private Tween _creditsCanvasGroupTween;
        private Tween _animateCardsTween;
        private Tween _scaleCardsTween;

        private TextAsset _jsonFile;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(HandlePlayButtonPressed);
            _creditsButton.onClick.AddListener(HandleCreditsButtonPressed);
            _endButton.onClick.AddListener(HandleEndButtonPressed);

            _allahAnimator = _allah.GetComponent<Animator>();
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(HandlePlayButtonPressed);
            _creditsButton.onClick.RemoveListener(HandleCreditsButtonPressed);
            _endButton.onClick.RemoveListener(HandleEndButtonPressed);
        }

        private void HandleEndButtonPressed()
        {
            _character1.ResetCharacter(_gameCanvasGroup.transform);
            _character2.ResetCharacter(_gameCanvasGroup.transform);

            _endgameCanvasGroup.gameObject.SetActive(false);

            _gameCanvasGroup.gameObject.SetActive(false);
            _creditsCanvasGroup.gameObject.SetActive(false);

            _backgroundMusicController.FadeoutMusic();

            _menuCanvasGroup.gameObject.SetActive(true);
        }

        public void Init()
        {
            if(null == _jsonFile)
                _jsonFile = Resources.Load<TextAsset>("options"); // No .json extension

            LoadOptions(_jsonFile.text);

            _endgameCanvasGroup.gameObject.SetActive(false);
            _endgameOverlayAnimator.gameObject.SetActive(false);

            _creditsCanvasGroup.gameObject.SetActive(false);

            _gameOver = false;

            _character1.transform.SetParent(_gameCanvasGroup.transform);
            _character2.transform.SetParent(_gameCanvasGroup.transform);

            _character1.OnGameOverReady -= HandleGameOverReady;
            _character1.OnGameOverReady += HandleGameOverReady;

            _character2.OnGameOverReady -= HandleGameOverReady;
            _character2.OnGameOverReady += HandleGameOverReady;

            _cardController.transform.GetChild(0).localScale = Vector3.zero;

            int gamesPlayed = PlayerPrefs.GetInt("games_played");

            PlayerPrefs.SetInt("games_played", ++gamesPlayed);

            PlayerPrefs.Save();

            SetDefaults();

            StartRound();
        }

        private void HandleGameOverReady()
        {
            _endgameCanvasGroup.gameObject.SetActive(true);

            DOTween.Kill(_endgameCanvasGroupTween);
            _endgameCanvasGroupTween = _endgameCanvasGroup.DOFade(1F, 1F).SetLink(gameObject);
        }

        public void SetCharacters(CharacterOption character1, CharacterOption character2)
        {
            _character1.Init(character1);
            _character2.Init(character2);

            _character1.transform.SetParent(_gameCanvasGroup.transform);
            _character2.transform.SetParent(_gameCanvasGroup.transform);

            //flip character
            if (_character2.transform.localScale.x >= 0)
            {
                _character2.transform.localScale = new Vector3(-_character2.transform.localScale.x, _character2.transform.localScale.y, _character2.transform.localScale.z);
            }
        }

        public void SetStage(StageOption stageOption)
        {
            float aspect = (float)Screen.width / Screen.height;
            aspect = Mathf.Round(aspect * 10f) / 10f; // Round to nearest 0.1
            float aspectThreshold = 16f / 9f;
            aspectThreshold = Mathf.Round(aspectThreshold * 10f) / 10f;

            //Wider than 16:9
            if (aspect > aspectThreshold)
            {
                _stage.sprite = stageOption.Sprite_219;
            }
            else
            {
                _stage.sprite = stageOption.Sprite;
            }
        }

        public void AllahIsThinking()
        {
            if (null != _currentDelayTween && _currentDelayTween.IsActive())
            {
                _currentDelayTween.Kill();
            }

            DOTween.Kill(_playAllahSoundTween);
            DOTween.Kill(_currentDelayTween);

            _allah.SetText();

            _playAllahSoundTween = DOVirtual.DelayedCall(2F, _allah.PlayThinkingSound).SetLink(gameObject);

            _currentDelayTween = DOVirtual.DelayedCall(4F, CalculateResult).SetLink(gameObject);
        }

        private void HandleCreditsButtonPressed()
        {
            _creditsCanvasGroup.gameObject.SetActive(true);

            DOTween.Kill(_creditsCanvasGroupTween);
            _creditsCanvasGroupTween = _creditsCanvasGroup.DOFade(1F,1F).SetLink(gameObject);

            _backgroundMusicController.PlayCreditsMusic();
        }

        private void SetDefaults()
        {
            _playButton.gameObject.SetActive(false);
            _allah.gameObject.SetActive(false);
        }

        private void SetAllah(bool state)
        {
            _allah.gameObject.SetActive(true);

            if (state)
            {
                _allahAnimator.SetTrigger("Descend");
            }
            else
            {
                _allahAnimator.SetTrigger("Ascend");
            }
        }

        private void CalculateResult()
        {
            bool syntaxCheck = CheckSyntax();

            float probability = (syntaxCheck) ? .55F : .2f;

            float result = Random.Range(0, 1F);

            if (result <= probability)
            {
                WinResult();
            }
            else
            {
                LoseResult();
            }

            DOTween.Kill(_waitBeforeHidingAllahTween);

            _waitBeforeHidingAllahTween = DOVirtual.DelayedCall(2F, WaitBeforeHidingAllah);
        }

        private void WaitBeforeHidingAllah()
        {
            SetAllah(false);

            if (!_gameOver)
            {
                _roundBackgroundOverlayAnimator.SetTrigger("FadeIn");

                StartRound();
            }
            else
            {
                _roundBackgroundOverlayAnimator.gameObject.SetActive(false);
            }
        }

        private void StartRound()
        {
            if (_gameOver)
                return;

            _cardController.ResetCards();

            _skippedShowingAllOptions = false;

            _optionsChosen = new GameObject[3];

            CreateOptions();

            DOTween.Kill(_animateCardsTween);
            _animateCardsTween = DOVirtual.DelayedCall(1F, WaitBeforeAnimatingOptions).SetLink(gameObject);

            _playButton.gameObject.SetActive(false);
        }

        private void WaitBeforeAnimatingOptions()
        {
            AnimateOptions(new Vector3(1.197272F, 1.197272F, 1.197272F));
        }

        private void AnimateOptions(Vector3 scale)
        {
            _as.PlayOneShot(_cardsDeltSFX);

            DOTween.Kill(_scaleCardsTween);
            _scaleCardsTween = _cardController.transform.GetChild(0).DOScale(scale, 0.6f)
                .SetLink(gameObject)
                .SetEase(Ease.OutBounce);
        }

        private void HandlePlayButtonPressed()
        {
            _skippedShowingAllOptions = true;

            _playButton.gameObject.SetActive(false);

            string answer = string.Empty;

            _as.PlayOneShot(_playButtonPressedSFX);

            SelectOpponentCharacterSpeech();

            StringBuilder sb = new StringBuilder();
            foreach (GameObject option in _optionsChosen)
            {
                OptionView optionView = option.GetComponent<OptionView>();

                sb.Append(optionView.Text.text);
                sb.Append(" ");
            }

            foreach (GameObject option in _options)
            { 
                if (_skippedShowingAllOptions)
                {
                    _skippedShowingAllOptions = false;
                }
            }

            _speachText1.fullText = sb.ToString();

            AnimateOptions(Vector3.zero);

            Sequence seq = DOTween.Sequence();

            seq.AppendInterval(2f);

            seq.AppendCallback(() => _character1.SetSpeechBubble(true));

            seq.AppendInterval(6f);

            seq.AppendCallback(() => _character2.SetSpeechBubble(true));

            seq.AppendInterval(6f);

            seq.AppendCallback(() => _roundBackgroundOverlayAnimator.SetTrigger("FadeOut"));

            seq.AppendCallback(() => SetAllah(true));

            seq.AppendInterval(1f);

            seq.AppendCallback(() => _character1.SetSpeechBubble(false));
            seq.AppendCallback(() => _character2.SetSpeechBubble(false));

            seq.SetLink(gameObject);

            seq.Play();
        }

        private void SelectOpponentCharacterSpeech()
        {
            OptionData firstOption = null;
            OptionData secondOption = null;
            OptionData thirdOption = null;

            ShuffleUtility.Shuffle(_options);

            foreach (var optionData in _optionData)
            {
                if(optionData.Type == OptionType.One)
                {
                    firstOption = optionData;
                }

                if (optionData.Type == OptionType.Two)
                {
                    secondOption = optionData;
                }

                if (optionData.Type == OptionType.Three)
                {
                    thirdOption = optionData;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(firstOption.Text);
            sb.Append(" ");
            sb.Append(secondOption.Text);
            sb.Append(" ");
            sb.Append(thirdOption.Text);

            _speachText2.fullText = sb.ToString();
        }

        private void LoadOptions(string json)
        {
            if(null == _optionData)
                _optionData = JsonConvert.DeserializeObject<List<OptionData>>(json);
        }

        private void CreateOptions()
        {
            ShuffleUtility.Shuffle(_optionData);

            int count = 0;
            int firstTopic = 0;
            int secondTopic = 0;
            int thirdTopic = 0;
            foreach (OptionData option in _optionData)
            {
                if(count >= 10)
                {
                    break;
                }

                if(option.Type == OptionType.One)
                {
                    if(firstTopic > 3)
                    {
                        continue;
                    }

                    firstTopic++;
                }
                if (option.Type == OptionType.Two)
                {
                    if (secondTopic > 3)
                    {
                        continue;
                    }

                    secondTopic++;
                }
                if (option.Type == OptionType.Three)
                {
                    if (thirdTopic > 3)
                    {
                        continue;
                    }

                    thirdTopic++;
                }

                var cards = _cardController.Cards;

                var card = cards[count++];

                var view = card.GetComponent<OptionView>();

                view.OnOptionSelected -= HandleOptionSelected;
                view.OnOptionSelected += HandleOptionSelected;

                view.OnOptionUnselected -= HandleOptionUnselected;
                view.OnOptionUnselected += HandleOptionUnselected;

                view.Init(option);

                _options.Add(card.gameObject);
            }
        }

        private void HandleOptionSelected(GameObject optionGameObject)
        {
            for (int i = 0; i < _optionsChosen.Length; i++)
            {
                if(null == _optionsChosen[i])
                {
                    _optionsChosen[i] = optionGameObject;

                    var view = _optionsChosen[i].GetComponent<OptionView>();
                    
                    if(i == 0)
                    {
                        view.SetSelectedFirst();
                    }
                    if (i == 1)
                    {
                        view.SetSelectedSecond();
                    }
                    if (i == 2)
                    {
                        view.SetSelectedThird();
                    }

                    LogState();
                    
                    CheckForFullOptions();

                    return;
                }
            }

            LogState();
        }

        private void CheckForFullOptions()
        {
            for (int i = 0; i < _optionsChosen.Length; i++)
            {
                if(null == _optionsChosen[i])
                {
                    _playButton.gameObject.SetActive(false);

                    return;
                }
            }

            _playButton.gameObject.SetActive(true);
        }

        private void HandleOptionUnselected(GameObject optionGameObject)
        {
            for (int i = 0; i < _optionsChosen.Length; i++)
            {
                if (null != _optionsChosen[i])
                {
                    if(_optionsChosen[i] == optionGameObject)
                    {
                        var view = _optionsChosen[i].GetComponent<OptionView>();

                        view.SetUnselected();
                        
                        _optionsChosen[i] = null;

                        LogState();

                        CheckForFullOptions();

                        return;
                    }
                }
            }

            LogState();
        }

        private void LoseResult()
        {
            _character2.SetLightning(false);

            _as.PlayOneShot(_thunderSFX);

            if (_character1.DoDamage())
            {
                GameOver(_character1);
            }
        }

        private void GameOver(Character character)
        {
            _gameOver = true;

            _endgameOverlayAnimator.transform.gameObject.SetActive(true);

            character.transform.SetParent(_endgameOverlayAnimator.transform);

            _endgameOverlayAnimator.SetTrigger("FadeOut");

            _backgroundMusicController.FadeoutMusic();
        }

        private void WinResult()
        {
            _character1.SetLightning(false);

            _as.PlayOneShot(_thunderSFX);

            if (_character2.DoDamage())
            {
                GameOver(_character2);
            }
        }

        private bool CheckSyntax()
        {
            bool syntaxCheck = true;

            for (int i = 0; i < _optionsChosen.Length; i++)
            {
                if (_optionsChosen[i].TryGetComponent<OptionView>(out OptionView optionView))
                {
                    if (i == 0)
                    {
                        if (optionView.OptionData.Type != OptionType.One)
                        {
                            syntaxCheck = false;
                            break;
                        }
                    }
                    if (i == 1)
                    {
                        if (optionView.OptionData.Type != OptionType.Two)
                        {
                            syntaxCheck = false;
                            break;
                        }
                    }
                    if (i == 2)
                    {
                        if (optionView.OptionData.Type != OptionType.Three)
                        {
                            syntaxCheck = false;
                            break;
                        }
                    }
                }
            }

            return syntaxCheck;
        }

        private void Update()
        {
            if(Keyboard.current.spaceKey.IsPressed())
            {
                //_character2.DoDamage();
            }
        }

        private void LogState()
        {
            for (int i = 0; i < _optionsChosen.Length; i++)
            {
                if (null == _optionsChosen[i])
                {
                    Debug.Log(i + ": Null");

                    continue;
                }

                Debug.Log(i + ": " + _optionsChosen[i].name);
            }
        }
    }
}