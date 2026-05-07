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
            fireParticleObject.SetActive(false);
    }

    private void Update()
    {

        if (fireParticleObject == null || shipTransform == null || targetPoint == null) return;

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
        if (isLookingAtTarget && !changed)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= time)
            {
                Characters[0].SetActive(false);
                Characters[1].SetActive(true);
                changed = true;
            }
        }
    }
}