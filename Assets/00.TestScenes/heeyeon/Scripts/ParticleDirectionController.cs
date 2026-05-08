using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ParticleDirectionController : MonoBehaviour
{
    [SerializeField] private GameObject fireParticleObject;
    [SerializeField] private Transform shipTransform;
    [SerializeField] private Transform targetPoint; // 바라볼 목표 위치
    [SerializeField] private float angleThreshold = 45f; // 이 각도 안에 있으면 불 ON

    [SerializeField] private float time = 6.0f;
    [SerializeField] private bool changed = false;
    float currentTime = 0.0f;
    public List<GameObject> Characters;

    private void Start()
    {
        if (fireParticleObject != null)
        {
            SetAllChildEffectsActive(false); // 하위 객체들까지 확실히 꺼둠
            fireParticleObject.SetActive(false);
        }
    }

    private void Update()
    {

        if (fireParticleObject == null || shipTransform == null || targetPoint == null || changed) return;
        // 배 → 목표 방향 벡터 (수평)
        Vector3 toTarget = targetPoint.position - shipTransform.position;
        toTarget.y = 0;
        toTarget.Normalize();

        // 배의 정면 방향 (수평)
        Vector3 shipForward = -shipTransform.right;
        shipForward.y = 0;
        shipForward.Normalize();

        // 두 방향 사이 각도
        float angle = Vector3.Angle(shipForward, toTarget);

        bool isLookingAtTarget = angle < angleThreshold;

        // 불 SetActive 제어
        if (isLookingAtTarget != fireParticleObject.activeSelf)
            fireParticleObject.SetActive(isLookingAtTarget);

        // 캐릭터 전환 타이머 (불 뿜는 동안만 카운트)
        if (isLookingAtTarget)
        {
            currentTime += Time.deltaTime;

            // 아직 실행되지 않았고(!changed), 시간이 다 됐을 때만 딱 한 번 실행
            if (currentTime >= time && !changed)
            {
                ExecuteOneShotEffects(); // 일괄 실행 함수 호출
                changed = true;          // 이후 Update 로직 완전히 차단
            }
        }
    }

    private void ExecuteOneShotEffects()
    {
        Debug.Log("[ParticleController] 최종 One-Shot 효과 실행");

        // 캐릭터 전환
        if (Characters != null && Characters.Count >= 2)
        {
            Characters[0].SetActive(false);
            Characters[1].SetActive(true);
        }

        // 3. GeoBukSeon_Effects 하위에 있는 모든 오디오와 파티클 찾아 실행
        if (fireParticleObject != null)
        {
            // 이펙트 실행 전, 혹시 꺼져있을 자식 오브젝트들을 켜줍니다.
            SetAllChildEffectsActive(true);

            // AudioSource 실행
            AudioSource[] audios = fireParticleObject.GetComponentsInChildren<AudioSource>(true);
            foreach (var audio in audios)
            {
                audio.Play(); // PlayOneShot 대신 Play() 사용 (Loop꺼짐 확인필수)
                Debug.Log($"[VFX] 오디오 재생: {audio.clip?.name}");
            }

            // ParticleSystem 실행
            ParticleSystem[] particles = fireParticleObject.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in particles)
            {
                ps.Play();
                Debug.Log($"[VFX] 파티클 재생: {ps.gameObject.name}");
            }

            // 4. 이펙트 재생이 끝나는 시점에 오브젝트를 숨기기 위해 코루틴 호출
            // 파티클 중 가장 긴 지속시간을 찾거나 정해진 시간 뒤에 끕니다.
            StartCoroutine(DisableEffectsAfterDelay(3.0f)); // 3초 뒤에 꺼짐
        }
    }

    private System.Collections.IEnumerator SwitchCharacterDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        Characters[0].SetActive(false);
        Characters[1].SetActive(true);
        Debug.Log("[ParticleController] 캐릭터 전환 완료");
    }

    // 하위 오디오/파티클 오브젝트들의 SetActive 상태를 일괄 제어하는 헬퍼 함수
    private void SetAllChildEffectsActive(bool active)
    {
        // ParticleSystem이 붙은 모든 자식 오브젝트 꺼기/켜기
        ParticleSystem[] particles = fireParticleObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in particles)
        {
            ps.gameObject.SetActive(active);
        }

        // AudioSource가 붙은 모든 자식 오브젝트 꺼기/켜기
        AudioSource[] audios = fireParticleObject.GetComponentsInChildren<AudioSource>(true);
        foreach (var audio in audios)
        {
            audio.gameObject.SetActive(active);
        }
    }

    // 일정 시간 뒤에 이펙트 부모 오브젝트를 꺼버리는 코루틴
    private System.Collections.IEnumerator DisableEffectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (fireParticleObject != null)
        {
            fireParticleObject.SetActive(false);
            Debug.Log("[ParticleController] 최종 효과 재생 완료 후 숨김");
        }
    }
}