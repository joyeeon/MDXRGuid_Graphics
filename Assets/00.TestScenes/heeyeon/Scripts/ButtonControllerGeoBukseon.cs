using System.Collections;
using TMPro;
using UnityEngine;

public class ButtonControllerGeoBukseon : MonoBehaviour
{
    [Header("Button 1 - 이순신 등장")]
    [SerializeField] private GameObject YisunsinBefore;


    [Header("Button 2 - 포즈 변경 & 진격 & 일본군 등장")]
    [SerializeField] private GameObject YisunsinAfter;
    [SerializeField] private GameObject japaneseArmy_Before;
    [SerializeField] private AudioClip DolgeokClip;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private TextMeshProUGUI BtnText;


    [Header("Button 3 - 거북선 VFX & 효과음")]
    [SerializeField] private GameObject geoBukSeonEffects;
    [SerializeField] private AudioClip explosionClip;
    private ParticleSystem[] vfxList;
    //public ParticleSystem test;

    [Header("첫 나레이션")]
    [SerializeField] private NarrationPlayer narrationPlayer;
    [SerializeField] private AudioSource sfxSource;      // 효과음 전용

    public int currentStep = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        geoBukSeonEffects.SetActive(true);
        Text.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        geoBukSeonEffects.SetActive(true);
        YisunsinBefore.SetActive(false);
        YisunsinAfter.SetActive(false);
        japaneseArmy_Before.SetActive(false);
        vfxList = geoBukSeonEffects.GetComponentsInChildren<ParticleSystem>(true);
    }

    public void OnbuttonClic()
    {
        switch(currentStep)
        {
            case 0:
                OnButton1Clicked();
                break;
            case 1:
                OnButton2Clicked();
                break;
            case 2:
                OnButton3Clicked();
                break;
        }

        if(currentStep < 2)
            currentStep++;
        
    }

    public void OnButton1Clicked()
    {
        if (YisunsinBefore != null)
            YisunsinBefore.SetActive(true);
        narrationPlayer.PlayNarration();

        BtnText.SetText("버튼을 한 번 더 눌러 보세요.");
    }

    public void OnButton2Clicked()
        {
        YisunsinBefore.SetActive(false);
        YisunsinAfter.SetActive(true);
        japaneseArmy_Before.SetActive(true);

        narrationPlayer.Pause();
        sfxSource.PlayOneShot(DolgeokClip);
        StartCoroutine(ResumeAfterClip(DolgeokClip.length));
        Debug.Log("Button 2 clicked: Changed pose, showed Japanese army, and played sound");
        Text.gameObject.SetActive(true);

        BtnText.SetText("버튼을 누르면 공격합니다.");
    }


    public void OnButton3Clicked()
    {
        Debug.Log("Button 3 clicked: Playing VFX and sound");
        //test.Play();
        foreach (var vfx in vfxList)
        {
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            vfx.Play();
        }
        sfxSource.PlayOneShot(explosionClip, 0.35f);
    }

    private IEnumerator ResumeAfterClip(float delay)
    {
        yield return new WaitForSeconds(delay);
        narrationPlayer.Resume();
    }
}
