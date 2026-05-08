using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ParticleDirectionControllerPos : MonoBehaviour
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

        // 거리를 먼저 체크 (예: 5미터 이내일 때만 작동)
        float distance = Vector3.Distance(shipTransform.position, targetPoint.position);
        if (distance > 5.0f)
        {
            currentTime = 0; // 멀어지면 타이머 리셋
            fireParticleObject.SetActive(false);
            return;
        }

        // 1. 배에서 목표 지점을 향하는 진짜 직선 벡터 (수평)
        Vector3 directionToTarget = (targetPoint.position - shipTransform.position);
        directionToTarget.y = 0; // 높이 무시
        directionToTarget.Normalize();

        // 2. 배의 실제 정면 벡터 (수평)
        // 기존에 -shipTransform.right를 사용하셨으니 그대로 유지하거나 transform.forward로 변경 가능합니다.
        Vector3 shipForward = shipTransform.forward;
        shipForward.y = 0;
        shipForward.Normalize();

        // 3. 두 직선이 얼마나 일치하는지 계산 (각도)
        float angle = Vector3.Angle(shipForward, directionToTarget);

        // 4. 설정한 오차 범위(angleThreshold) 내에 들어오면 마주 보는 것으로 간주
        bool isLookingAtTarget = angle < angleThreshold;

        if (isLookingAtTarget)
        {
            currentTime += Time.deltaTime;

            // [디버그용] 현재 몇 초 동안 조준 중인지 콘솔에 찍어보세요
            // Debug.Log($"조준 중: {currentTime}초");

            if (currentTime >= time && !changed)
            {
                ExecuteOneShotEffects();
                changed = true;
            }
        }
        else
        {
            // 핵심: 조준이 빗나가는 순간 타이머를 0으로 초기화합니다.
            // 이렇게 하면 시작할 때 잠깐 마주쳤더라도 다시 0부터 세게 됩니다.
            currentTime = 0f;

            if (fireParticleObject.activeSelf)
                fireParticleObject.SetActive(false);
        }
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
}