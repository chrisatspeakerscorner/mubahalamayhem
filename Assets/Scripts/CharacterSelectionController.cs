using EasyTransition;
using Mubahala.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class CharacterOption
{
    public string Name;

    public Sprite Happy;
    public Sprite Angry;
    public Sprite Sad;

    public string Name_Atr;
    public string From_Atr;
    public string Speciality_Atr;

    public List<AudioClip> selectedSFX;
    public List<AudioClip> tauntSFX;

    public AudioClip endGameSFX;

    public int audioIndex;

    public void Init()
    {
        audioIndex = Random.Range(0, selectedSFX.Count - 1);
    }
}

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] private Transform _characterSelectionGrid;

    [SerializeField] private List<GameObject> _portraits;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _firstButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private GameController _gameController;
    [SerializeField] private Canvas _sceneSelectorCanvas;
    [SerializeField] private Canvas _menuCanvas;

    [SerializeField] private List<CharacterOption> _characters;

    [SerializeField] private TextMeshProUGUI _nameAtr;
    [SerializeField] private TextMeshProUGUI _fromAtr;
    [SerializeField] private TextMeshProUGUI _specialityAtr;

    [SerializeField] private TransitionSettings _transitionSettings;

    private TransitionManager _transitionManager;

    [SerializeField] private AudioSource _as;

    [SerializeField] private AudioClip _transitionSwipe;
    [SerializeField] private AudioClip _clickedSFX;

    [SerializeField] private GameObject _attributesBoard;
    [SerializeField] private GameObject _selectYourCharacter;
    [SerializeField] private GameObject _defaultLogo;

    private CharacterOption _character_1;
    private CharacterOption _character_2;

    private Dictionary<Button, GameObject> _dictionary;

    private int _selectedSFXCounter = 0;

    private void Awake()
    {
        _playButton.onClick.AddListener(HandlePlayButtonPressed);
        _backButton.onClick.AddListener(HandleBackButtonPressed);

        foreach(CharacterOption co in _characters)
        {
            co.Init();
        }

        _playButton.interactable = false;

        _attributesBoard.SetActive(false);

        _selectYourCharacter.SetActive(true);
        _defaultLogo.SetActive(true);

        _selectedSFXCounter = Random.Range(0, 3);

        _dictionary = new();

        int counter = 0;
        foreach (Transform child in _characterSelectionGrid)
        {
            var button = child.GetComponentInChildren<Button>();

            if(_portraits.Count > counter)
            {
                _dictionary.Add(button, _portraits[counter]);
            }

            counter++;
            button.onClick.AddListener(() => HandleCharacterSelected(button));
        }
    }

    private void Start()
    {
        _transitionManager = TransitionManager.Instance();
    }

    private void HandleBackButtonPressed()
    {
        _menuCanvas.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }

    private void HandlePlayButtonPressed()
    {
        _transitionManager.onTransitionCutPointReached -= OpenStageSelectionScreen;
        _transitionManager.onTransitionCutPointReached += OpenStageSelectionScreen;

        SelectRandomCharacter();

        _gameController.SetCharacters(_character_1, _character_2);

        _as.PlayOneShot(_transitionSwipe);

        _transitionManager.Transition(_transitionSettings, 0F);
    }

    private void OpenStageSelectionScreen()
    {
        _sceneSelectorCanvas.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }

    private void SelectRandomCharacter()
    {
        int rand = (int)UnityEngine.Random.Range(0, _portraits.Count);

        _character_2 = _characters[rand];
    }

    private void HandleCharacterSelected(Button button)
    {
        if(_dictionary.TryGetValue(button, out GameObject portrait))
        {
            var match = _characters.FirstOrDefault(obj => obj.Name == button.transform.parent.name);

            if(_character_1 == match)
            {
                return;
            }

            foreach (GameObject go in _portraits)
            {
                go.SetActive(false);
            }

            _character_1 = match;

            portrait.SetActive(true);

            var clip = GetSelectedSFX(match);

            _as.Stop();

            _as.PlayOneShot(_clickedSFX);

            _as.PlayOneShot(clip);

            _playButton.interactable = true;

            _attributesBoard.SetActive(true);

            _selectYourCharacter.SetActive(false);
            _defaultLogo.SetActive(false);

            _nameAtr.text = $"<b>NAME:</b> {_character_1.Name_Atr}";
            _fromAtr.text = $"<b>FROM:</b> {_character_1.From_Atr}";
            _specialityAtr.text = $"<b>SPECIALITY:</b> {_character_1.Speciality_Atr}";
        }
    }

    private AudioClip GetSelectedSFX(CharacterOption co)
    {
        if (co.selectedSFX.Count == 0)
            return null;

        co.audioIndex++;

        if(co.audioIndex > co.selectedSFX.Count - 1)
        {
            co.audioIndex = 0;
        }

        if(co.selectedSFX.Count > co.audioIndex && null != co.selectedSFX[co.audioIndex])
        {
            return co.selectedSFX[co.audioIndex];
        }

        return null;
    }
}
