using EasyTransition;
using Mubahala.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StageOption
{
    public string Name;
    public Sprite Sprite;
    public Sprite Sprite_219;
}

public class StageSelectionController : MonoBehaviour
{
    [SerializeField] private Transform _stageSelectionGrid;

    [SerializeField] private List<GameObject> _portraits;

    [SerializeField] private GameController _gameController;

    [SerializeField] private Canvas _gameCanvas;
    [SerializeField] private Canvas _characterSelectCanvas;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _firstButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private List<StageOption> _stages;

    [SerializeField] private TransitionSettings _transitionSettings;

    [SerializeField] private AudioSource _as;

    [SerializeField] private AudioClip _transitionSwipe;

    private TransitionManager _transitionManager;

    private StageOption _spriteOption;

    private Dictionary<Button, GameObject> _dictionary;

    private void Awake()
    {
        _playButton.onClick.AddListener(HandlePlayButtonPressed);
        _backButton.onClick.AddListener(HandleBackButtonPressed);

        _dictionary = new();

        int counter = 0;
        foreach (Transform child in _stageSelectionGrid)
        {
            var button = child.GetComponent<Button>();

            if (_portraits.Count > counter)
            {
                _dictionary.Add(button, _portraits[counter]);
            }

            counter++;

            button.onClick.AddListener(() => HandleStageSelected(button));
        }

        HandleStageSelected(_firstButton);
    }

    private void Start()
    {
        _transitionManager = TransitionManager.Instance();
    }

    private void HandleBackButtonPressed()
    {
        _characterSelectCanvas.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }

    private void HandleStageSelected(Button button)
    {
        foreach (GameObject go in _portraits)
        {
            go.SetActive(false);
        }

        if (_dictionary.TryGetValue(button, out GameObject portrait))
        {
            var match = _stages.FirstOrDefault(obj => obj.Name == button.name);

            _spriteOption = match;

            SetStage(_spriteOption, portrait.GetComponentInChildren<Image>());

            portrait.SetActive(true);
        }
    }

    private void HandlePlayButtonPressed()
    {
        _transitionManager.onTransitionCutPointReached -= OpenGameScreen;
        _transitionManager.onTransitionCutPointReached += OpenGameScreen;

        _gameController.SetStage(_spriteOption);

        _gameController.Init();

        _as.PlayOneShot(_transitionSwipe);

        _transitionManager.Transition(_transitionSettings, 0F);
    }

    public void SetStage(StageOption stageOption, Image portrait)
    {
        float aspect = (float)Screen.width / Screen.height;
        aspect = Mathf.Round(aspect * 10f) / 10f; // Round to nearest 0.1
        float aspectThreshold = 16f / 9f;
        aspectThreshold = Mathf.Round(aspectThreshold * 10f) / 10f;

        //Wider than 16:9
        if (aspect > aspectThreshold)
        {
            portrait.sprite = stageOption.Sprite_219;
        }
        else
        {
            portrait.sprite = stageOption.Sprite;
        }
    }

    private void OpenGameScreen()
    {
        _gameCanvas.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }
}
