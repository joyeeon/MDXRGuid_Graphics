// NarrationPlayer.cs
using UnityEngine;
using System.Collections;

public class NarrationPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip narration1;
    [SerializeField] private AudioClip narration2;
    [SerializeField] private AudioSource narrationSource;


    private void Awake()
    {
        narrationSource = GetComponent<AudioSource>();
    }

    public void PlayNarration()
    {
        StartCoroutine(PlaySequence());
    }

    public void Pause() => narrationSource.Pause();
    public void Resume() => narrationSource.UnPause();

    private IEnumerator PlaySequence()
    {
        narrationSource.clip = narration1;
        narrationSource.Play();
        yield return new WaitForSeconds(narration1.length);

        narrationSource.clip = narration2;
        narrationSource.Play();
    }
}