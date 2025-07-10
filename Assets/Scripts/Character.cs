using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class Character : MonoBehaviour
{
    public Action OnGameOverReady;
    
    [SerializeField] private float _maxHealth;

    [SerializeField] private Image _characterImage;
    [SerializeField] private RawImage _lightningImage;

    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private ParticleSystem _ps;

    [SerializeField] private Animator _speechBubbleAnimator;

    [SerializeField] private GameObject _speechbubble;
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private AudioSource _as;
    [SerializeField] private AudioClip _speechSoundSFX;
    [SerializeField] private AudioClip _landslideSFX;

    private float _health;

    private Vector3 _startingPosition;

    private Tween _moveToCenterTween;
    private Tween _startTypingTween;
    private Tween _playSpeechTween;
    private Tween _characterSpeechTween;

    private Material _flashMat;
    private Tween _flashTween;

    private CharacterOption _characterOption;

    private int _currentTauntIndex;

    private void Awake()
    {
        _speechbubble.gameObject.SetActive(false);

        float randomTime = Random.Range(0f, 1f);

        _characterAnimator.Play("Idle", 0, randomTime);

        _flashMat = Instantiate(_characterImage.material); // clone to avoid affecting shared material

        _characterImage.material = _flashMat;
    }

    public void ResetCharacter(Transform parent)
    {
        _rectTransform.anchoredPosition = _startingPosition;
        _rectTransform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        _rectTransform.transform.SetParent(parent);

        _rectTransform.transform.SetSiblingIndex(9);
    }

    public void Init(CharacterOption characterOption)
    {
        _health = _maxHealth;

        _characterOption = characterOption;

        _startingPosition = _rectTransform.anchoredPosition;

        _characterImage.sprite = _characterOption.Happy;

        _currentTauntIndex = Random.Range(0, _characterOption.tauntSFX.Count - 1);  
        
        if(characterOption.Name.Equals("godlogic",StringComparison.OrdinalIgnoreCase))
        {
            transform.localScale = new Vector3(1.2F, 1.2F, 1.2F);
        }
    }

    public void SetSpeechBubble(bool state)
    {
        _speechbubble.SetActive(true);

        if (state)
        {
            _speechBubbleAnimator.SetTrigger("FadeIn");

            var textAnimator = _speechbubble.GetComponentInChildren<TextAnimator>();

            DOTween.Kill(_startTypingTween);
            DOTween.Kill(_playSpeechTween);

            _startTypingTween = DOVirtual.DelayedCall(.5F, textAnimator.StartTyping).SetLink(gameObject);
            _playSpeechTween = DOVirtual.DelayedCall(.5F, PlaySpeechSound).SetLink(gameObject);
        }
        else
        {
            var text = _speechbubble.GetComponentInChildren<TextMeshProUGUI>();
            text.text = string.Empty;

            _speechBubbleAnimator.SetTrigger("FadeOut");
        }
    }

    private void PlaySpeechSound()
    {
        _as.PlayOneShot(_speechSoundSFX);
    }

    private void PlayTauntSound()
    {
        if(_currentTauntIndex > _characterOption.tauntSFX.Count)
        {
            _currentTauntIndex = 0;
        }

        AudioClip taunt = _characterOption.tauntSFX[_currentTauntIndex++];

        _as.PlayOneShot(taunt);
    }

    public void SetLightning(bool state)
    {
        _lightningImage.enabled = state;
    }

    public bool DoDamage()
    {
        SetLightning(true);

        _ps.Play();

        _health--;

        FlashWhite();

        if (_health < 1)
        {
            Lost();

            return true;
        }

        DOVirtual.DelayedCall(1.25F, PlayTauntSound);

        if (_health == 1)
        {
            _characterImage.sprite = _characterOption.Sad;
        }
        if (_health == 2)
        {
            _characterImage.sprite = _characterOption.Angry;
        }
        if (_health >= 3)
        {
            _characterImage.sprite = _characterOption.Happy;
        }

        return false;
    }

    private void FlashWhite()
    {
        _flashTween?.Kill();
        _flashMat.SetFloat("_FlashAmount", 1f);

        _flashTween = DOTween.To(
            () => _flashMat.GetFloat("_FlashAmount"),
            x => _flashMat.SetFloat("_FlashAmount", x),
            0f,
            1f
        ).SetEase(Ease.Linear).SetLink(gameObject);
    }

    private void Lost()
    {
        MoveToCenter();
    }

    private void MoveToCenter()
    {
        var rectTransform = GetComponent<RectTransform>();

        RectTransform parent = rectTransform.parent as RectTransform;

        // Calculate center position relative to anchor
        Vector2 parentSize = parent.rect.size;
        Vector2 targetPosition = new Vector2();

        // Adjust target position based on horizontal anchoring
        if (rectTransform.anchorMin.x == 0 && rectTransform.anchorMax.x == 0)
        {
            // Left-aligned anchoring – shift left
            targetPosition.x += parentSize.x / 2f;
        }
        else if (rectTransform.anchorMin.x == 1 && rectTransform.anchorMax.x == 1)
        {
            // Right-aligned anchoring – shift right
            targetPosition.x -= parentSize.x / 2f;
        }

        DOTween.Kill(_moveToCenterTween);

        // Move to center of parent (Canvas) after 2 seconds
        _moveToCenterTween = rectTransform.DOAnchorPos(targetPosition, 1f)
                     .SetDelay(2f)
                     .SetEase(Ease.InOutQuad)
                     .SetLink(gameObject)
                     .OnComplete(PlayEndGameSFX);
    }

    private void PlayEndGameSFX()
    {
        if (null == _characterOption.endGameSFX)
        {
            Shake();
        }
        else
        {
            if(_characterOption.Name.Equals("PaulW", StringComparison.OrdinalIgnoreCase))
            {
                if(PlayerPrefs.GetInt("games_played") < 3)
                {
                    Shake();

                    return;
                }
            }

            _as.PlayOneShot(_characterOption.endGameSFX);

            DOTween.Kill(_characterSpeechTween);
            _characterSpeechTween = DOVirtual.DelayedCall(_characterOption.endGameSFX.length, Shake).SetLink(gameObject);
        }
    }

    private void Shake()
    {
        var rectTransform = GetComponent<RectTransform>();
        var imageRectTransform = _characterImage.GetComponent<RectTransform>();

        RectTransform parent = rectTransform.parent as RectTransform;

        // Calculate center position relative to anchor
        Vector2 parentSize = parent.rect.size;
        Vector2 targetPosition = new Vector2(0F, -parentSize.y * .8F);

        Sequence seq = DOTween.Sequence();

        seq.Append(rectTransform.DOShakeAnchorPos(
            duration: 5F,           // Duration of the shake
            strength: new Vector2(30, 30), // Intensity (X/Y shake in units)
            vibrato: 50,              // How many shakes per second
            randomness: 90,           // How much randomness (higher = more chaotic)
            snapping: false,
            fadeOut: true
        ).SetLink(gameObject)
        .SetDelay(3F));
        seq.Join(imageRectTransform.DOAnchorPos(targetPosition, 5F)
                     .SetEase(Ease.InOutQuad)
                     .SetLink(gameObject)
                     .OnStart(PlayDescentSound)
                     .OnComplete(Finished));

        seq.Play();
    }

    private void PlayDescentSound()
    {
        _as.PlayOneShot(_landslideSFX);
    }

    private void Finished()
    {
        OnGameOverReady?.Invoke();
    }
}
