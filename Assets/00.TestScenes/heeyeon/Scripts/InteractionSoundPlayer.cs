using UnityEngine;

public class InteractionSoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("사운드 설정")]
    public AudioClip interactionClip; // 재생할 mp3 파일
    [Range(0f, 1f)]
    public float volume = 0.5f;       // 소리 크기

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound()
    {
        if (interactionClip != null)
        {
            audioSource.clip = interactionClip;
            audioSource.volume = volume;
            audioSource.Stop();
            audioSource.Play();
        }
    }
}