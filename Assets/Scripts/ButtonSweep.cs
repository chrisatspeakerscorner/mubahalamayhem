using DG.Tweening;
using UnityEngine;

public class ButtonSweep : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        DOVirtual.DelayedCall(5F, OnSweep, false);
    }

    private void OnSweep()
    {
        _animator.SetTrigger("Sweep");

        DOVirtual.DelayedCall(5F, OnSweep, false);
    }
}
