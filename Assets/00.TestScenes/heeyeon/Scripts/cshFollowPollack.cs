using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class cshFollowPollack : MonoBehaviour
{
    public Transform Pollack;
    public Transform mouth;
    public float facingThreshold = 0.95f;
    public float detectionDistance = 20f;
    public float followSpeed = 0.08f;
    public float acceleration = 1.2f;
    public float arrivalDistance = 0.1f;

    public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public float minScaleRatio = 0f;

    private bool isFollow = false;
    private bool isPreparing = false;     // VFX 켜지고 대기 중인 상태
    public bool IsFollowing => isFollow;


    private float currentSpeed;
    public AudioClip followSound;
    private Vector3 initialScale;
    private float initialDistance;

    public float vfxToFollowDelay = 1.0f;

    private bool hasTriggeredVFX = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialScale = transform.localScale;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Pollack == null) return;

        if (!isFollow && !isPreparing)
            CheckFacingPollack();
        else if (isFollow)
            MoveTowardPollack();
    }

    void CheckFacingPollack()
    {

        if (PollackVFXManager.Instance != null && PollackVFXManager.Instance.IsSomeoneFollowing)
            return; // 다른 누군가가 이미 따라가는 중이라면 새로 시작하지 않음
        Vector3 toPollack = transform.position - Pollack.position;

        if (toPollack.magnitude > detectionDistance) return;

        float dot = Vector3.Dot(-Pollack.transform.right, toPollack.normalized);
        if (dot < facingThreshold) return;

        isPreparing = true;

        // 매니저에게 이펙트를 켜달라고 요청, 따라오기 방지 
        if (PollackVFXManager.Instance != null)
        {
            PollackVFXManager.Instance.IsSomeoneFollowing = true; // 다른 오브젝트가 따라오는 중임을 알림
            PollackVFXManager.Instance.StartVFX();
            hasTriggeredVFX = true;
        }

        StartCoroutine(PrepareFollow());
    }

    private System.Collections.IEnumerator PrepareFollow()
    {
        yield return new WaitForSeconds(vfxToFollowDelay);

        currentSpeed = followSpeed;
        initialDistance = Vector3.Distance(transform.position, mouth.position);
        if (initialDistance < 0.0001f) initialDistance = 0.0001f;

        isPreparing = false;
        isFollow = true;

        if (followSound != null)
            AudioSource.PlayClipAtPoint(followSound, transform.position, 0.5f);
    }

    void MoveTowardPollack()
    {
        if (mouth == null) { isFollow = false; return; }

        float distance = Vector3.Distance(transform.position, mouth.position);

        if (distance < arrivalDistance)
        {
            transform.localScale = initialScale * minScaleRatio;
            OnArrive();
            return;
        }

        currentSpeed += acceleration * Time.deltaTime;
        transform.position = Vector3.MoveTowards(
            transform.position,
            mouth.position,
            currentSpeed * Time.deltaTime
        );

        float progress = 1f - Mathf.Clamp01(distance / initialDistance);
        float curveValue = scaleCurve.Evaluate(progress);
        float scaleRatio = Mathf.Lerp(minScaleRatio, 1f, curveValue);
        transform.localScale = initialScale * scaleRatio;
    }

    void OnArrive()
    {
        isFollow = false;
        Destroy(gameObject);
    }

    // 오브젝트가 파괴될 때 (OnArrive로 파괴되든, 외부 요인으로 파괴되든) 이펙트 카운트 감소
    void OnDisable()
    {
        // 내가 이펙트를 켠 적이 있다면 카운트를 줄임
        if (hasTriggeredVFX && PollackVFXManager.Instance != null)
        {
            PollackVFXManager.Instance.StopVFX();
            PollackVFXManager.Instance.IsSomeoneFollowing = false; // 따라오는 중이 아님을 알림
            hasTriggeredVFX = false; // 중복 실행 방지 안전장치
        }
    }

}
