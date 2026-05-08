using UnityEngine;

public class PollackVFXManager : MonoBehaviour
{
    public static PollackVFXManager Instance; // 싱글톤 접근

    public GameObject warningVFX;
    public bool IsSomeoneFollowing { get; set; } = false;
    private int suckingCount = 0; // 현재 빨려가고 있는 객체의 수

    void Awake()
    {
        // 어디서든 쉽게 접근할 수 있도록 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (warningVFX != null)
            warningVFX.SetActive(false);
    }

    // 객체가 빨려가기 시작할 때 호출
    public void StartVFX()
    {
        suckingCount++;
        // 하나라도 빨려가기 시작하면 이펙트를 켭니다.
        if (suckingCount == 1 && warningVFX != null)
        {
            warningVFX.SetActive(true);
        }
    }

    // 객체가 도착하거나 파괴될 때 호출
    public void StopVFX()
    {
        suckingCount--;
        // 빨려가는 객체가 하나도 없을 때만 이펙트를 끕니다.
        if (suckingCount <= 0)
        {
            suckingCount = 0; // 안전 장치
            if (warningVFX != null)
            {
                warningVFX.SetActive(false);
            }
        }
    }


}
