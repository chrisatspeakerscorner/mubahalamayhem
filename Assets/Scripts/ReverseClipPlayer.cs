using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class ReverseClipPlayer : MonoBehaviour
{
    public AnimationClip clip; // Assign in Inspector

    private PlayableGraph graph;
    private AnimationClipPlayable playable;

    public void PlayBackward()
    {
        graph = PlayableGraph.Create("ReverseClipGraph");
        var output = AnimationPlayableOutput.Create(graph, "Animation", GetComponent<Animator>());

        playable = AnimationClipPlayable.Create(graph, clip);
        playable.SetSpeed(-1);
        playable.SetTime(clip.length); // Start at the end

        DOTween.Kill(gameObject);
        DOVirtual.DelayedCall(clip.length, Finished);

        output.SetSourcePlayable(playable);
        graph.Play();
    }

    private void Finished()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
        if (graph.IsValid()) graph.Destroy();
    }

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
        if (graph.IsValid()) graph.Destroy();
    }
}
