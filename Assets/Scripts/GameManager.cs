using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public SpawnManager spawnManager; // SpawnManager 스크립트 참조

    // 이벤트 정의
    public event EventHandler OnGameStart; // 게임 시작 이벤트

    void Start()
    {
        StartCoroutine(StartGameAfterAnimation());

        OnGameStart += spawnManager.StartSpawning; // 이벤트 구독
    }

    private IEnumerator StartGameAfterAnimation()
    {
        // 애니메이션이 끝날 때까지 대기 (예: 3초)
        yield return new WaitForSeconds(3f);

        // GameStartEvent();
        OnGameStart?.Invoke(this, EventArgs.Empty); // 이벤트 발생
        Debug.Log("GameStartEvent 호출됨!");
    }

    void Update()
    {
        // 모바일 화면 터치 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {

            }
        }
    }
}
