using UnityEngine;

public class FloatingGhost : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.2f;      // 떠다니는 속도
    [SerializeField] private float floatHeight = 0.015f;   // 위아래 이동 범위
    [Header("유령 회전 설정")]
    [SerializeField] private float yRotationRange = 3f;  // 좌우 도리도리 흔들림 각도 (Y축)
    [SerializeField] private float yRotationSpeed = 1.5f; // 좌우 흔들림 속도
    [SerializeField] private float zRotationRange = 3f;   // 좌우 기우뚱 기우뚱 각도 (Z축)
    [SerializeField] private float zRotationSpeed = 2f;   // 기우뚱 속도

    private cshFollowPollack followPollack;

    private Rigidbody rb;

    private Quaternion _startRot;
    private float _timeCounter;

    private void Start()
    {
        _startRot = transform.localRotation; // 초기 회전값 저장
        followPollack = GetComponent<cshFollowPollack>();
        rb = GetComponent<Rigidbody>();

        
        _timeCounter = Random.Range(0f, 100f);
    }

    private void Update()
    {

        if (followPollack!=null && followPollack.IsFollowing)
        {
            return;
        }
        // 사인파로 위아래 이동
        if (rb != null && rb.isKinematic)
        {
            // 잡혀서 이동하는 동안에는 현재 회전값을 기준점으로 계속 갱신해 줍니다.
            _startRot = transform.localRotation;
            return;
        }

        _timeCounter += Time.deltaTime;

        // 3. 고정 좌표가 아니라 '수학적 변화량'만 계산해서 현재 위치에 더해줍니다.
        // 이 방식을 쓰면 손을 놓은 그 자리에서 다시 자연스럽게 둥실둥실 춤을 춥니다.
        float wave = Mathf.Sin(_timeCounter * floatSpeed) * floatHeight;

        // 위아래 미세한 움직임 적용 (X, Z는 현재 위치 유지)
        transform.position = new Vector3(transform.position.x, transform.position.y + wave * Time.deltaTime * 10f, transform.position.z);

        // 4. 회전 효과 적용
        float sinY = Mathf.Sin(_timeCounter * yRotationSpeed) * yRotationRange;
        float sinZ = Mathf.Sin(_timeCounter * zRotationSpeed) * zRotationRange;

        transform.localRotation = _startRot * Quaternion.Euler(0, sinY, sinZ);
    }
}