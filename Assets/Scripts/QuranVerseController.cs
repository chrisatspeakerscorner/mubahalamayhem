using System.Collections.Generic;
using UnityEngine;

public class QuranVerseController : MonoBehaviour
{
    [TextArea]
    [SerializeField] private List<string> _quranVerses;

    [SerializeField] private TextAnimator _textAnimator;

    private int _randomSetIndex = -1;

    private void OnEnable()
    {
        if(_randomSetIndex == -1)
        {
            ShowRandomQuranVerse();
        }
        else
        {
            ShowNextQuranVerse();
        }
    }

    public void ShowRandomQuranVerse()
    {
        _randomSetIndex = Random.Range(0, _quranVerses.Count);

        _textAnimator.fullText = _quranVerses[_randomSetIndex];

        _textAnimator.StartTyping();
    }

    public void ShowNextQuranVerse()
    {
        ++_randomSetIndex;

        if(_randomSetIndex >= _quranVerses.Count)
        {
            _randomSetIndex = 0;
        }

        _textAnimator.fullText = _quranVerses[_randomSetIndex];

        _textAnimator.StartTyping();
    }
}
