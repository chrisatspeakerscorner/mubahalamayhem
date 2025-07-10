using DG.Tweening;
using Mubahala.Controller;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Allah : MonoBehaviour
{
    [Serializable]
    public class AllahsOption
    {
        public AudioClip _allahsVoiceSFX;

        public string _allahsSpeech;
    }

    [SerializeField] private GameController _controller;

    [SerializeField] private AudioSource _as;

    [SerializeField] private TextMeshProUGUI _allahsSpeech;

    [SerializeField] private List<AllahsOption> _allahsOptions;

    [TextArea]
    [SerializeField] private string _allahsDefaultSpeech;

    private AllahsOption _option;

    public void SetText()
    {
        SetOption();

        _allahsSpeech.text = _allahsDefaultSpeech;

        _allahsSpeech.color = new Color(_allahsSpeech.color.r, _allahsSpeech.color.g, _allahsSpeech.color.b, 0F);

        _allahsSpeech.DOFade(1F, 1F);
    }

    private void SetOption()
    {
        int index = Random.Range(0, _allahsOptions.Count);

        _option = _allahsOptions[index];
    }

    public void PlayThinkingSound()
    {
        _allahsSpeech.text = _option._allahsSpeech;

        _as.PlayOneShot(_option._allahsVoiceSFX);
    }

    public void AnimationFinished()
    {
        _controller.AllahIsThinking();
    }
}
