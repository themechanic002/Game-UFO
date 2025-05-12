using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public SpawnManager spawnManager; // SpawnManager 스크립트 참조

    bool isGameStarted = false; // 게임 시작 여부

    // 이벤트 정의
    public event EventHandler OnGameStart; // 게임 시작 이벤트

    void Start()
    {
        StartCoroutine(StartGameAfterAnimation());

        isGameStarted = false; // 게임 시작 여부 초기화

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

}
