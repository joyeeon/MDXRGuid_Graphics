using System.Collections;
using UnityEngine;

public class InteractionSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("사운드 설정")]
    public AudioClip interactionClip; // 재생할 mp3 파일
    [Range(0f, 1f)]
    public float volume = 0.5f;       // 소리 크기

    private NarrationManager narrationManager;
    private Coroutine resumeCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        narrationManager = FindObjectOfType<NarrationManager>();
    }

    public void PlaySound()
    {
        if (interactionClip != null)
        {
            // 기존 재개 코루틴 취소
            if (resumeCoroutine != null)
                StopCoroutine(resumeCoroutine);

            // 나레이션 일시정지
            if (narrationManager != null && narrationManager.IsPlaying)
                narrationManager.PauseNarration();

            audioSource.clip = interactionClip;
            audioSource.volume = volume;
            audioSource.Stop();
            audioSource.Play();

            // 재생 끝나면 나레이션 재개
            resumeCoroutine = StartCoroutine(ResumeAfterSound());
        }
    }

    private IEnumerator ResumeAfterSound()
    {
        yield return new WaitForSeconds(interactionClip.length);
        narrationManager?.ResumeNarration();
    }
}