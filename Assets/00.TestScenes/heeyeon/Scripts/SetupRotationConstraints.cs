using UnityEngine;
using Oculus.Interaction;

public class RotationConstraintSetup : MonoBehaviour
{
    private Quaternion lockedRotation;

    private void Start()
    {
        // 시작 시 회전값 저장
        lockedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // 매 프레임 Y축 회전만 허용하고 나머지 고정
        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(
            lockedRotation.eulerAngles.x,  // X 고정
            euler.y,                        // Y 자유
            lockedRotation.eulerAngles.z   // Z 고정
        );
    }
}