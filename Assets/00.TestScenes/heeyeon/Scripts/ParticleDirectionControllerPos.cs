using UnityEngine;
using System.Collections.Generic;

public class LineCrossDetector : MonoBehaviour
{
    [SerializeField] private Transform lineObject;      // 기준이 되는 빈 객체
    [SerializeField] private Transform shipTransform;   // 배
    //[SerializeField] private GameObject vfxPrefab;      // VFX 프리팹
    [SerializeField] private float vfxDuration = 3.0f; // VFX 지속 시간


    public List<GameObject> Characters;

    private bool changed = false;
    private float previousLocalX;

    [SerializeField] private ButtonControllerGeoBukseon buttonController;

    private void Start()
    {
        // 시작 시 배의 초기 로컬 X 위치 저장
        previousLocalX = GetLocalX();
    }

    private void Update()
    {
        if (changed || lineObject == null || shipTransform == null) return;

        float currentLocalX = GetLocalX();
        Debug.Log($"localX: {currentLocalX}");

        bool crossed = previousLocalX > 0f && currentLocalX <= 0f;

        if (crossed && buttonController.currentStep > 1)
        {
            ExecuteOneShotEffects();
            changed = true;
            Debug.Log("crossed! changed = true");
        }

        previousLocalX = currentLocalX;
    }

    private float GetLocalX()
    {
        // 빈 객체의 로컬 공간 기준으로 배의 위치 변환
        return lineObject.InverseTransformPoint(shipTransform.position).x;
    }

    private void ExecuteOneShotEffects()
    {
        if (buttonController.currentStep <= 1) return;
        if (Characters != null && Characters.Count >= 2 )
        {
            Characters[0].SetActive(false);
            Characters[1].SetActive(true);
        }
        buttonController.OnButton3Clicked();

       /* if (vfxPrefab != null)
            vfxPrefab.SetActive(true);*/
    }

    /*private System.Collections.IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (vfxPrefab != null)
            vfxPrefab.SetActive(false);
    }*/
}