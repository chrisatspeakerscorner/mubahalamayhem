using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingImageLooper : MonoBehaviour
{
    public RectTransform imageRect;
    public SpriteLibrary spriteLibrary;
    public Image imageComponent;

    public float duration = 5f;

    public float delay = 0F;

    private float topY = 1133F;
    private float bottomY = -1133F;
    private Tween Tween;

    void Start()
    {
        StartFloatingLoop();
    }

    void StartFloatingLoop()
    {
        // Set a random sprite initially
        imageComponent.sprite = GetRandomSprite();

        // Set start position at the top
        imageRect.anchoredPosition = new Vector2(imageRect.anchoredPosition.x, topY);

        // Animate downward
        Tween.Kill();
        Tween = imageRect.DOAnchorPosY(bottomY, duration)
                 .SetEase(Ease.Linear)
                 .SetDelay(delay)
                 .OnComplete(() =>
                 {
                     // Loop: pick new sprite, reset position, and restart
                     imageComponent.sprite = GetRandomSprite();
                     ContinueFloatingLoop();
                 });
    }

    void ContinueFloatingLoop()
    {
        // Set start position at the top
        imageRect.anchoredPosition = new Vector2(imageRect.anchoredPosition.x, topY);

        // Animate downward
        Tween.Kill();
        Tween = imageRect.DOAnchorPosY(bottomY, duration)
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                     // Loop: pick new sprite, reset position, and restart
                     imageComponent.sprite = GetRandomSprite();
                     ContinueFloatingLoop();
                 });
    }

    Sprite GetRandomSprite()
    {
        return spriteLibrary.Sprites[Random.Range(0, spriteLibrary.Sprites.Count)];
    }
}
