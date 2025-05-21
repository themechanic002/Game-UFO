using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeverManager : MonoBehaviour
{
  private static FeverManager instance;
  public static FeverManager Instance
  {
    get
    {
      if (instance == null)
      {
        instance = FindObjectOfType<FeverManager>();
        if (instance == null)
        {
          GameObject obj = new GameObject("FeverManager");
          instance = obj.AddComponent<FeverManager>();
        }
      }
      return instance;
    }
  }

  [SerializeField] private Slider feverGauge;
  [SerializeField] private AudioClip feverStartSound;    // Fever 시작 효과음
  [SerializeField] private AudioClip feverEndSound;      // Fever 종료 효과음
  private AudioSource audioSource;                       // 오디오 소스
  private float perfectFillAmount = 0.1f; // Perfect 판정당 채워지는 게이지 양 (10번이면 꽉 참)
  private float feverDuration = 5f; // Fever 지속 시간
  private bool isFeverActive = false;
  public bool IsFeverActive => isFeverActive;
  private Coroutine feverCoroutine;

  private GameObject suckUpPoint;                        // SuckUpPoint 오브젝트
  private Vector3 originalScale;                         // SuckUpPoint의 원래 크기
  private const float FEVER_SCALE_MULTIPLIER = 2f;     // Fever 시 크기 증가 배수
  private const float SCALE_ANIMATION_DURATION = 0.5f;   // 크기 변화 애니메이션 시간
  private SpriteRenderer suckUpPointRenderer;           // SuckUpPoint의 스프라이트 렌더러
  private Color originalColor;                          // SuckUpPoint의 원래 색상
  private readonly Color FEVER_COLOR = new Color(0.5f, 0.8f, 1f, 1f); // Fever 시 색상 (하늘색)

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }

    // 오디오 소스 컴포넌트 추가
    audioSource = gameObject.AddComponent<AudioSource>();
  }

  void Start()
  {
    if (feverGauge == null)
    {
      feverGauge = GameObject.Find("FeverGauge").GetComponent<Slider>();
    }
    feverGauge.value = 0f;

    // SuckUpPoint 초기화
    suckUpPoint = GameObject.FindGameObjectWithTag("SuckUpPoint");
    if (suckUpPoint != null)
    {
      originalScale = suckUpPoint.transform.localScale;
      suckUpPointRenderer = suckUpPoint.GetComponent<SpriteRenderer>();
      if (suckUpPointRenderer != null)
      {
        originalColor = suckUpPointRenderer.color;
      }
    }
  }

  public void OnPerfectJudgment()
  {
    if (!isFeverActive)
    {
      feverGauge.value += perfectFillAmount;
      if (feverGauge.value >= 1f)
      {
        StartFever();
      }
    }
  }

  public void OnGoodJudgment()
  {
    // Good 판정은 게이지 유지
  }

  public void OnMissJudgment()
  {
    if (!isFeverActive)
    {
      feverGauge.value = 0f;
    }
  }

  private void StartFever()
  {
    if (feverCoroutine != null)
    {
      StopCoroutine(feverCoroutine);
    }
    feverCoroutine = StartCoroutine(FeverRoutine());
    PlayFeverStartSound();
    StartCoroutine(ScaleSuckUpPoint(true));
  }

  private void PlayFeverStartSound()
  {
    if (audioSource != null && feverStartSound != null)
    {
      audioSource.PlayOneShot(feverStartSound);
    }
  }

  private void PlayFeverEndSound()
  {
    if (audioSource != null && feverEndSound != null)
    {
      audioSource.PlayOneShot(feverEndSound);
    }
  }

  private IEnumerator ScaleSuckUpPoint(bool isExpanding)
  {
    if (suckUpPoint == null || suckUpPointRenderer == null) yield break;

    Vector3 startScale = suckUpPoint.transform.localScale;
    Vector3 targetScale = isExpanding ?
        new Vector3(originalScale.x * FEVER_SCALE_MULTIPLIER, originalScale.y, originalScale.z) :
        originalScale;

    Color startColor = suckUpPointRenderer.color;
    Color targetColor = isExpanding ? FEVER_COLOR : originalColor;

    float elapsedTime = 0f;
    while (elapsedTime < SCALE_ANIMATION_DURATION)
    {
      elapsedTime += Time.deltaTime;
      float t = elapsedTime / SCALE_ANIMATION_DURATION;

      // 크기와 색상을 동시에 변화
      suckUpPoint.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
      suckUpPointRenderer.color = Color.Lerp(startColor, targetColor, t);

      yield return null;
    }

    suckUpPoint.transform.localScale = targetScale;
    suckUpPointRenderer.color = targetColor;
  }

  private IEnumerator FeverRoutine()
  {
    isFeverActive = true;
    feverGauge.value = 1f;

    // TODO: 여기에 Fever 효과 발동 시의 추가 효과 구현
    Debug.Log("Fever 효과 발동!");

    float elapsedTime = 0f;
    while (elapsedTime < feverDuration)
    {
      elapsedTime += Time.deltaTime;
      feverGauge.value = 1f - (elapsedTime / feverDuration);
      if (suckUpPointRenderer != null)
      {
        suckUpPointRenderer.color = FEVER_COLOR;
      }
      yield return null;
    }

    feverGauge.value = 0f;
    isFeverActive = false;
    PlayFeverEndSound();
    StartCoroutine(ScaleSuckUpPoint(false));
  }
}