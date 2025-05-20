using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 이동하는 오브젝트를 관리하는 컨트롤러
/// - 오브젝트 이동
/// - SuckUpPoint와의 상호작용
/// - 판정 및 점수 처리
/// </summary>
public class ObjectController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float speed = 5f;            // 이동 속도
    [SerializeField] private float destroyXPosition = -10f; // 파괴될 X 위치

    [Header("판정 설정")]
    [SerializeField] private float perfectRange = 0.2f;   // Perfect 판정 범위
    [SerializeField] private float goodRange = 0.5f;      // Good 판정 범위

    [Header("흡입 애니메이션 설정")]
    [SerializeField] private float abductionSpeed = 5f;   // 흡입 애니메이션 속도
    [SerializeField] private float stretchFactor = 3f;    // 늘어나는 정도
    [SerializeField] private float maxStretchHeight = 3f; // 최대 늘어나는 높이

    // 상태 변수
    private bool isOnSuckUpPoint = false;    // SuckUpPoint와 겹쳐있는지 여부
    private bool suckedUp = false;           // 흡입되었는지 여부
    private bool hasPassedUFO = false;       // UFO를 지나쳤는지 여부
    private bool hasShownMiss = false;       // Miss 판정을 표시했는지 여부
    private bool isBeingAbducted = false;    // 흡입 애니메이션 중인지 여부

    // 참조 변수
    private GameObject UFO;                  // UFO 오브젝트
    private Vector3 ufoPosition;             // UFO의 위치
    private Vector3 originalScale;           // 원래 크기
    private Vector3 originalPosition;        // 원래 위치
    private TextMeshProUGUI scoreText;       // 점수 텍스트
    private SuckUpController suckUpController; // SuckUpController 참조
    private GameManager gameManager;         // GameManager 참조

    void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// 필요한 컴포넌트들을 초기화
    /// </summary>
    private void InitializeComponents()
    {
        // UFO 오브젝트 찾기
        UFO = GameObject.Find("UFO");
        if (UFO == null)
        {
            Debug.LogError("UFO 오브젝트를 찾을 수 없습니다.");
            return;
        }
        ufoPosition = UFO.transform.position;

        // SuckUpController 찾기
        suckUpController = GameObject.FindObjectOfType<SuckUpController>();
        if (suckUpController == null)
        {
            Debug.LogError("SuckUpController를 찾을 수 없습니다.");
        }

        // GameManager 찾기
        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager를 찾을 수 없습니다.");
        }

        // 원래 크기와 위치 저장
        originalScale = transform.localScale;
        originalPosition = transform.position;

        InitializeScore();
    }

    /// <summary>
    /// 점수 텍스트 컴포넌트 초기화
    /// </summary>
    private void InitializeScore()
    {
        scoreText = GameObject.Find("Score")?.GetComponent<TextMeshProUGUI>();
        if (scoreText == null)
        {
            Debug.LogError("점수 텍스트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (!suckedUp)
        {
            MoveObject();
            CheckPassedUFO();
        }

        CheckDestroyCondition();
    }

    /// <summary>
    /// 오브젝트를 왼쪽으로 이동
    /// </summary>
    private void MoveObject()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    /// <summary>
    /// UFO를 지나쳤는지 확인
    /// </summary>
    private void CheckPassedUFO()
    {
        if (!hasPassedUFO && transform.position.x < ufoPosition.x)
        {
            hasPassedUFO = true;
        }
    }

    /// <summary>
    /// 오브젝트가 파괴되어야 하는지 확인
    /// </summary>
    private void CheckDestroyCondition()
    {
        if (transform.position.x < destroyXPosition)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 판정을 계산하고 표시
    /// </summary>
    private void ShowJudgment()
    {
        string judgment;
        Color color;

        if (suckedUp)
        {
            CalculateSuccessJudgment(out judgment, out color);
        }
        else
        {
            judgment = "Miss";
            color = new Color(0.8f, 0.2f, 0.2f);
            FeverManager.Instance.OnMissJudgment();
        }

        JudgmentManager.Instance.ShowJudgment(judgment, color);
    }

    /// <summary>
    /// 성공 판정(Perfect/Good) 계산
    /// </summary>
    private void CalculateSuccessJudgment(out string judgment, out Color color)
    {
        float distance = Mathf.Abs(transform.position.x - ufoPosition.x);
        if (distance <= perfectRange)
        {
            judgment = "Perfect!";
            color = new Color(1f, 0.92f, 0.016f);
            FeverManager.Instance.OnPerfectJudgment();
            UpdateScore(2);
        }
        else
        {
            judgment = "Good!";
            color = new Color(0.2f, 0.8f, 0.2f);
            FeverManager.Instance.OnGoodJudgment();
            UpdateScore(1);
        }
    }

    /// <summary>
    /// 점수 업데이트
    /// </summary>
    private void UpdateScore(int baseScore)
    {
        if (scoreText != null)
        {
            int currentScore = int.Parse(scoreText.text);
            int scoreToAdd = FeverManager.Instance.IsFeverActive ? baseScore * 2 : baseScore;
            scoreText.text = (currentScore + scoreToAdd).ToString();
        }
    }

    /// <summary>
    /// SuckUpPoint와 겹치기 시작할 때 호출
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SuckUpPoint"))
        {
            isOnSuckUpPoint = true;
            if (suckUpController != null && !SuckUpController.IsInCooldown())
            {
                suckUpController.currentObject = gameObject;
            }
            Debug.Log("오브젝트가 SuckUpPoint와 겹치기 시작했습니다: " + gameObject.name);
        }
    }

    /// <summary>
    /// SuckUpPoint에서 벗어날 때 호출
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SuckUpPoint"))
        {
            isOnSuckUpPoint = false;
            if (suckUpController != null && suckUpController.currentObject == gameObject)
            {
                suckUpController.currentObject = null;
            }
            Debug.Log("오브젝트가 SuckUpPoint에서 벗어났습니다: " + gameObject.name);
            CheckMissJudgment();
        }
    }

    /// <summary>
    /// Miss 판정 확인
    /// </summary>
    private void CheckMissJudgment()
    {
        if (hasPassedUFO && !hasShownMiss && !suckedUp)
        {
            ShowJudgment();
            hasShownMiss = true;
            if (gameManager != null)
            {
                gameManager.DecreaseLife();
            }
        }
    }

    /// <summary>
    /// 흡입 시작
    /// </summary>
    public void StartSucking()
    {
        if (isOnSuckUpPoint && !suckedUp)
        {
            Debug.Log("오브젝트 흡입을 시작합니다: " + gameObject.name);
            suckedUp = true;
            ShowJudgment();
            StartAbduction(ufoPosition);
        }
        else
        {
            Debug.Log("오브젝트를 흡입할 수 없습니다: " + gameObject.name +
                     " (SuckUpPoint 겹침: " + isOnSuckUpPoint +
                     ", 이미 흡입됨: " + suckedUp + ")");
        }
    }

    /// <summary>
    /// 흡입 애니메이션 시작
    /// </summary>
    private void StartAbduction(Vector3 ufoPosition)
    {
        if (!isBeingAbducted)
        {
            isBeingAbducted = true;
            StartCoroutine(AbductionAnimation(ufoPosition));
        }
    }

    /// <summary>
    /// 흡입 애니메이션 코루틴
    /// </summary>
    private IEnumerator AbductionAnimation(Vector3 ufoPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;

        // UFO 위치를 약간 위로 조정
        Vector3 targetPosition = ufoPosition + Vector3.up * 0.5f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * abductionSpeed;
            float t = Mathf.Clamp01(elapsedTime);

            // 위치 이동
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // 스케일 조정 (위로 늘어나는 효과)
            float currentStretch = Mathf.Lerp(1f, stretchFactor, t);
            float currentHeight = Mathf.Lerp(0f, maxStretchHeight, t);

            transform.localScale = new Vector3(
                originalScale.x / currentStretch,
                originalScale.y * currentStretch,
                originalScale.z
            );

            // 투명도 조정
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0.5f, t);
                spriteRenderer.color = color;
            }

            yield return null;
        }

        // 애니메이션 완료 후 오브젝트 제거
        Destroy(gameObject);
    }
}
