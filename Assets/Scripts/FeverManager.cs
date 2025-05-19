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
  private float perfectFillAmount = 0.1f; // Perfect 판정당 채워지는 게이지 양 (10번이면 꽉 참)
  private float feverDuration = 5f; // Fever 지속 시간
  private bool isFeverActive = false;
  public bool IsFeverActive => isFeverActive;
  private Coroutine feverCoroutine;

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
  }

  void Start()
  {
    if (feverGauge == null)
    {
      feverGauge = GameObject.Find("FeverGauge").GetComponent<Slider>();
    }
    feverGauge.value = 0f;
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
      yield return null;
    }

    feverGauge.value = 0f;
    isFeverActive = false;
  }
}