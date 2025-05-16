using UnityEngine;
using TMPro;

public class JudgmentManager : MonoBehaviour
{
  private static JudgmentManager instance;
  public static JudgmentManager Instance
  {
    get
    {
      if (instance == null)
      {
        instance = FindObjectOfType<JudgmentManager>();
        if (instance == null)
        {
          GameObject obj = new GameObject("JudgmentManager");
          instance = obj.AddComponent<JudgmentManager>();
        }
      }
      return instance;
    }
  }

  private TextMeshProUGUI judgmentText;
  private float judgmentDisplayTime = 1f;
  private float judgmentTimer = 0f;

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
    // 판정 텍스트 오브젝트 찾기
    GameObject judgmentObj = GameObject.Find("JudgmentText");
    if (judgmentObj != null)
    {
      judgmentText = judgmentObj.GetComponent<TextMeshProUGUI>();
      if (judgmentText != null)
      {
        judgmentText.gameObject.SetActive(false);
      }
    }
  }

  void Update()
  {
    // 판정 텍스트 타이머 업데이트
    if (judgmentText != null && judgmentText.gameObject.activeSelf)
    {
      judgmentTimer += Time.deltaTime;
      if (judgmentTimer >= judgmentDisplayTime)
      {
        judgmentText.gameObject.SetActive(false);
        judgmentTimer = 0f;
      }
    }
  }

  public void ShowJudgment(string judgment, Color color)
  {
    if (judgmentText == null) return;

    // 새로운 판정이 들어오면 이전 판정을 즉시 교체
    judgmentText.text = judgment;
    judgmentText.color = color;
    judgmentText.gameObject.SetActive(true);
    judgmentTimer = 0f;
  }
}