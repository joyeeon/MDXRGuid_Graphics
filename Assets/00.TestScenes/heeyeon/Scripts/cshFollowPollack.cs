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
    private AudioSource audioSource;
    private Vector3 initialScale;
    private float initialDistance;


    public GameObject warningVFX;        // VisualEffect → GameObject로 변경
    public float vfxToFollowDelay = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        // 시작 시 VFX 프리팹 비활성화
        if (warningVFX != null)
            warningVFX.gameObject.SetActive(false);
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
        Vector3 toPollack = transform.position - Pollack.position;

        if (toPollack.magnitude > detectionDistance) return;

        float dot = Vector3.Dot(-Pollack.transform.right, toPollack.normalized);
        if (dot < facingThreshold) return;

        if (warningVFX != null)
            warningVFX.SetActive(true);

        isPreparing = true;
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

        if (audioSource != null && audioSource.clip != null)
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position, 0.5f);
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

        if (warningVFX != null)
            warningVFX.SetActive(false);

        Destroy(gameObject);
    }

}
