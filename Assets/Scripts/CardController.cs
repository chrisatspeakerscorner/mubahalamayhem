using Mubahala.View;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CardController : MonoBehaviour
{
    public List<OptionView> Cards => _cards;

    [SerializeField] private List<OptionView> _cards;

    public @InputSystem_Actions _inputActions;

    private Vector2 _swipeStartPos;
    private float _swipeThreshold = 50f; // Minimum distance in pixels for a swipe

    private OptionView _currentCard;
    private OptionView _previousCard;

    private int _currentCardIndex = 4; // midcard

    [SerializeField] private AudioSource _as;

    [SerializeField] private AudioClip _cardChangeSFX;

    void Awake()
    {
        _inputActions = new @InputSystem_Actions();

        EnhancedTouchSupport.Enable(); // Enable Enhanced Touch for WebGL/mobile

        _currentCard = _cards[_currentCardIndex];
    }

    private void OnEnable()
    {
        _currentCard = _cards[_currentCardIndex];

        _inputActions.Enable();

        _inputActions.Player.Left_card.performed += ctx => OnMoveLeft();
        _inputActions.Player.Right_card.performed += ctx => OnMoveRight();

        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;

        foreach (OptionView view in _cards)
        {
            view.SetInteractable(false);
        }

        ShowCard();
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Player.Left_card.performed -= ctx => OnMoveLeft();
        _inputActions.Player.Right_card.performed -= ctx => OnMoveRight();

        _currentCardIndex = 4;

        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerUp -= OnFingerUp;
    }

    public void ResetCards()
    {
        foreach(OptionView card in _cards)
        {
            Animator cardAnimator = card.GetComponent<Animator>();

            cardAnimator.ResetTrigger("Background");
            cardAnimator.ResetTrigger("Flip");
        }
    }

    private void OnFingerDown(Finger finger)
    {
        _swipeStartPos = finger.screenPosition;
    }

    private void OnFingerUp(Finger finger)
    {
        Vector2 swipeEndPos = finger.screenPosition;
        float deltaX = swipeEndPos.x - _swipeStartPos.x;

        if (Mathf.Abs(deltaX) >= _swipeThreshold)
        {
            if (deltaX > 0)
                OnMoveRight();
            else
                OnMoveLeft();
        }
    }

    private void ShowCard()
    {
        if(null != _previousCard)
        {
            Animator cardAnimator = _previousCard.GetComponent<Animator>();

            cardAnimator.SetTrigger("Background");

            _previousCard.SetInteractable(false);
        }

        _currentCard = _cards[_currentCardIndex];

        var animator = _currentCard.GetComponent<Animator>();

        animator.ResetTrigger("Background");

        animator.SetTrigger("Flip");

        _currentCard.transform.SetAsLastSibling();

        _currentCard.SetInteractable(true);

        _previousCard = _currentCard;
    }

    private void OnMoveLeft()
    {
        Debug.Log("Move Left Triggered");

        _currentCardIndex--;

        if(_currentCardIndex < 0)
        {
            _currentCardIndex = 0;
        }
        else
        {
            _as.PlayOneShot(_cardChangeSFX);

            ShowCard();
        }
    }

    private void OnMoveRight()
    {
        Debug.Log("Move Right Triggered");

        _currentCardIndex++;

        if (_currentCardIndex > 9)
        {
            _currentCardIndex = 9;
        }
        else
        {
            _as.PlayOneShot(_cardChangeSFX);

            ShowCard();
        }
    }
}
