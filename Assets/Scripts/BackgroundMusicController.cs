using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    [Serializable]
    public class BackgroundTrack
    {
        public string name;
        public AudioClip audioClip;
    }

    [SerializeField] private AudioSource _as;

    [SerializeField] private List<BackgroundTrack> _tracks;

    [SerializeField] private BackgroundTrack _creditsTrack;

    private int _randomIndex = -1;
    private bool _playingBackgroundMusic = false;

    private float _previousVolume;

    public void PlayMusic(string name)
    {
        foreach(BackgroundTrack track in _tracks)
        {
            if(track.name.Equals(name,StringComparison.OrdinalIgnoreCase))
            {
                _as.PlayOneShot(track.audioClip);

                _playingBackgroundMusic = true;
            }
        }
    }

    public void PlayCreditsMusic()
    {
        _playingBackgroundMusic = false;

        _as.Stop();

        _as.PlayOneShot(_creditsTrack.audioClip);
    }

    public void PlayMusicRandom()
    {
        _randomIndex = UnityEngine.Random.Range(0, _tracks.Count);

        _as.Stop();

        _as.PlayOneShot(_tracks[_randomIndex].audioClip);

        _playingBackgroundMusic = true;
    }

    public void StopMusic()
    {
        _playingBackgroundMusic = false;

        _as.Stop();
    }

    public void FadeoutMusic()
    {
        _previousVolume = _as.volume;

        _as.DOFade(0f, 1F).OnComplete(FinishedFadingOutMusic);
    }

    private void FinishedFadingOutMusic()
    {
        _as.volume = _previousVolume;

        _playingBackgroundMusic = false;

        _as.Stop();
    }

    private void NextTrack()
    {
        _randomIndex++;

        if(_randomIndex >= _tracks.Count)
        {
            _randomIndex = 0;
        }

        _as.PlayOneShot(_tracks[_randomIndex].audioClip);
    }

    private void Update()
    {
        if (!_playingBackgroundMusic)
            return;

        if (!_as.isPlaying)
        {
            NextTrack();
        }
    }
}
