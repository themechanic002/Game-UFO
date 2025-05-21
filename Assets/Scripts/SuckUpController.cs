using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UFO의 흡입 기능을 관리하는 컨트롤러
/// - 쿨타임 관리
/// - 오브젝트 흡입 처리
/// - UFO와 SuckUpPoint의 시각적 효과
/// </summary>
public class SuckUpController : MonoBehaviour
{
    [Header("쿨타임 설정")]
    private static float cooldownTime = 0.4f;        // 쿨타임 지속 시간
    private static float cooldownTimer = cooldownTime;  // 현재 쿨타임 타이머
    private static bool isInCooldown = false;      // 쿨타임 상태

    [Header("UFO 관련")]
    private static SpriteRenderer suckUpPointRenderer;  // SuckUpPoint의 스프라이트 렌더러
    private static Color originalSuckUpPointColor;      // SuckUpPoint의 원래 색상
    private static Vector3 originalUfoPosition;         // UFO의 원래 위치
    private static Vector3 originalSuckUpPosition;      // SuckUpPoint의 원래 위치
    private static GameObject suckUpPoint;              // SuckUpPoint 오브젝트
    private static GameObject UFO;                      // UFO 오브젝트

    [Header("효과음")]
    [SerializeField] private AudioClip cooldownSound;   // 쿨타임 효과음
    [SerializeField] private AudioClip popSound;        // 흡입 효과음

    [Header("흡입 설정")]
    [SerializeField] private float suckUpSpeed = 55f;   // 흡입 속도

    private AudioSource audioSource;                    // 오디오 소스
    public GameObject currentObject;                    // 현재 흡입 중인 오브젝트
    private bool isSucking = false;                     // 흡입 상태
    private float currentPopPitch = 0.5f;              // 현재 pop 효과음의 pitch
    private int perfectCount = 0;                      // 연속 perfect 횟수
    private const float MIN_POP_PITCH = 0.5f;          // 최소 pitch 값
    private const float MAX_POP_PITCH = 1.4f;          // 최대 pitch 값
    private const int MAX_PERFECT_COUNT = 10;          // 최대 perfect 연속 횟수
    private float pitchStep;                           // pitch 증가 단위

    void Start()
    {
        InitializeComponents();
        // pitch 증가 단위 계산
        pitchStep = (MAX_POP_PITCH - MIN_POP_PITCH) / MAX_PERFECT_COUNT;
    }

    /// <summary>
    /// 필요한 컴포넌트들을 초기화
    /// </summary>
    private void InitializeComponents()
    {
        // 변수 초기화
        cooldownTimer = cooldownTime;
        isInCooldown = false;

        // UFO 오브젝트 찾기
        UFO = GameObject.Find("UFO");
        if (UFO == null)
        {
            Debug.LogError("UFO 오브젝트를 찾을 수 없습니다.");
            return;
        }
        originalUfoPosition = UFO.transform.position;

        // SuckUpPoint 초기화
        suckUpPoint = GameObject.FindGameObjectWithTag("SuckUpPoint");
        if (suckUpPoint != null)
        {
            suckUpPointRenderer = suckUpPoint.GetComponent<SpriteRenderer>();
            if (suckUpPointRenderer != null)
            {
                originalSuckUpPointColor = suckUpPointRenderer.color;
            }
            originalSuckUpPosition = suckUpPoint.transform.position;
        }

        // 오디오 소스 컴포넌트 추가
        audioSource = gameObject.AddComponent<AudioSource>();
        if (popSound == null)
        {
            Debug.LogError("흡입 효과음이 할당되지 않았습니다.");
        }
    }

    void Update()
    {
        UpdateCooldown();      // 쿨타임 업데이트
        HandleTouchInput();    // 터치 입력 처리
        UpdateSucking();       // 흡입 상태 업데이트
    }

    /// <summary>
    /// 쿨타임 상태를 업데이트
    /// </summary>
    private void UpdateCooldown()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= 5f)
        {
            cooldownTimer = 5f;
        }

        if (isInCooldown && cooldownTimer > cooldownTime)
        {
            ResetCooldown();
        }
    }

    /// <summary>
    /// 쿨타임을 초기화하고 관련 효과를 제거
    /// </summary>
    private void ResetCooldown()
    {
        isInCooldown = false;
        if (suckUpPointRenderer != null)
        {
            suckUpPointRenderer.color = originalSuckUpPointColor;
        }
        ResetPositions();
    }

    /// <summary>
    /// UFO와 SuckUpPoint를 원래 위치로 복원
    /// </summary>
    private void ResetPositions()
    {
        if (UFO != null && UFO.transform.position != originalUfoPosition)
        {
            UFO.transform.position = originalUfoPosition;
        }
        if (suckUpPoint != null && suckUpPoint.transform.position != originalSuckUpPosition)
        {
            suckUpPoint.transform.position = originalSuckUpPosition;
        }
    }

    /// <summary>
    /// 터치 입력을 처리
    /// </summary>
    private void HandleTouchInput()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            if (isInCooldown)
            {
                HandleCooldownTouch();
            }
            else if (currentObject != null)
            {
                StartSucking();
            }
            StartCooldown();
        }
    }

    /// <summary>
    /// 쿨타임 중 터치 시 효과 처리
    /// </summary>
    private void HandleCooldownTouch()
    {
        if (suckUpPointRenderer != null)
        {
            suckUpPointRenderer.color = Color.Lerp(originalSuckUpPointColor, Color.red, 0.3f);
        }
        StartCoroutine(ShakeObjects());
        PlayCooldownSound();
    }

    /// <summary>
    /// 쿨타임 효과음 재생
    /// </summary>
    private void PlayCooldownSound()
    {
        if (audioSource != null && cooldownSound != null)
        {
            audioSource.PlayOneShot(cooldownSound);
        }
    }

    /// <summary>
    /// 쿨타임 시작
    /// </summary>
    private void StartCooldown()
    {
        cooldownTimer = 0f;
        isInCooldown = true;
    }

    /// <summary>
    /// 흡입 상태 업데이트
    /// </summary>
    private void UpdateSucking()
    {
        if (isSucking && currentObject != null)
        {
            MoveObjectToUFO();
            CheckObjectReachedUFO();
        }
    }

    /// <summary>
    /// 오브젝트를 UFO 방향으로 이동
    /// </summary>
    private void MoveObjectToUFO()
    {
        Vector3 direction = (UFO.transform.position - currentObject.transform.position).normalized;
        currentObject.transform.position += direction * suckUpSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 오브젝트가 UFO에 도달했는지 확인
    /// </summary>
    private void CheckObjectReachedUFO()
    {
        if (Vector3.Distance(currentObject.transform.position, UFO.transform.position) < 0.5f)
        {
            Destroy(currentObject);
            isSucking = false;
            currentObject = null;
        }
    }

    /// <summary>
    /// UFO와 SuckUpPoint를 진동시키는 코루틴
    /// </summary>
    private System.Collections.IEnumerator ShakeObjects()
    {
        float shakeDuration = 0.2f;    // 진동 지속 시간
        float shakeAmount = 0.1f;      // 진동 강도
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            UFO.transform.position = originalUfoPosition + new Vector3(x, y, 0);
            suckUpPoint.transform.position = originalSuckUpPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ResetPositions();
    }

    /// <summary>
    /// 현재 쿨타임 상태 반환
    /// </summary>
    public static bool IsInCooldown()
    {
        return isInCooldown;
    }

    /// <summary>
    /// 오브젝트 흡입 시작
    /// </summary>
    private void StartSucking()
    {
        if (currentObject != null)
        {
            ObjectController objectController = currentObject.GetComponent<ObjectController>();
            if (objectController != null)
            {
                Debug.Log("오브젝트 흡입을 시작합니다: " + currentObject.name);
                objectController.StartSucking();
                isSucking = true;
                PlayPopSound();
            }
            else
            {
                Debug.LogError("ObjectController 컴포넌트를 찾을 수 없습니다: " + currentObject.name);
            }
        }
        else
        {
            Debug.Log("흡입할 오브젝트가 없습니다.");
        }
    }

    /// <summary>
    /// 흡입 효과음 재생
    /// </summary>
    private void PlayPopSound()
    {
        if (audioSource != null && popSound != null)
        {
            audioSource.pitch = currentPopPitch;
            audioSource.PlayOneShot(popSound);
        }
    }

    /// <summary>
    /// Perfect 판정 시 pitch 증가
    /// </summary>
    public void OnPerfect()
    {
        perfectCount++;
        if (perfectCount <= MAX_PERFECT_COUNT)
        {
            currentPopPitch = MIN_POP_PITCH + (pitchStep * perfectCount);
        }
    }

    /// <summary>
    /// Good 판정 시 pitch 유지
    /// </summary>
    public void OnGood()
    {
        perfectCount = 0;
        currentPopPitch = MIN_POP_PITCH;
    }

    /// <summary>
    /// Miss 판정 시 pitch 초기화
    /// </summary>
    public void OnMiss()
    {
        perfectCount = 0;
        currentPopPitch = MIN_POP_PITCH;
    }
}
