using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public SpawnManager spawnManager; // SpawnManager 스크립트 참조

    [Header("게임 설정")]
    public int maxLife = 3;           // 최대 생명력
    private int currentLife;          // 현재 생명력

    [Header("하트 UI")]
    [SerializeField] private GameObject heart1;    // 첫 번째 하트
    [SerializeField] private GameObject heart2;    // 두 번째 하트
    [SerializeField] private GameObject heart3;    // 세 번째 하트

    bool isGameStarted = false;       // 게임 시작 여부

    // 이벤트 정의
    public event EventHandler OnGameStart;     // 게임 시작 이벤트
    public event EventHandler OnLifeDecreased; // 생명력 감소 이벤트
    public event EventHandler OnGameOver;      // 게임 오버 이벤트

    void Start()
    {
        StartCoroutine(StartGameAfterAnimation());

        isGameStarted = false;        // 게임 시작 여부 초기화
        currentLife = maxLife;        // 생명력 초기화
        UpdateHeartUI();              // 하트 UI 초기화

        // 이벤트 구독
        OnGameStart += spawnManager.StartSpawning;
    }

    private IEnumerator StartGameAfterAnimation()
    {
        // 애니메이션이 끝날 때까지 대기 (예: 3초)
        yield return new WaitForSeconds(3f);

        // GameStartEvent();
        OnGameStart?.Invoke(this, EventArgs.Empty); // 이벤트 발생
        isGameStarted = true; // 게임 시작 상태로 변경
        Debug.Log("GameStartEvent 호출됨!");
    }

    /// <summary>
    /// 생명력 감소
    /// </summary>
    public void DecreaseLife()
    {
        if (currentLife > 0)
        {
            currentLife--;
            UpdateHeartUI();          // 하트 UI 업데이트
            OnLifeDecreased?.Invoke(this, EventArgs.Empty);
            Debug.Log($"생명력 감소! 남은 생명력: {currentLife}");

            if (currentLife <= 0)
            {
                GameOver();
            }
        }
    }

    /// <summary>
    /// 하트 UI 업데이트
    /// </summary>
    private void UpdateHeartUI()
    {
        if (heart1 != null) heart1.SetActive(currentLife >= 1);
        if (heart2 != null) heart2.SetActive(currentLife >= 2);
        if (heart3 != null) heart3.SetActive(currentLife >= 3);
    }

    /// <summary>
    /// 게임 오버 처리
    /// </summary>
    private void GameOver()
    {
        isGameStarted = false;
        OnGameOver?.Invoke(this, EventArgs.Empty);
        Debug.Log("게임 오버!");
    }

    /// <summary>
    /// 현재 생명력 반환
    /// </summary>
    public int GetCurrentLife()
    {
        return currentLife;
    }
}
