using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectController : MonoBehaviour
{
    private bool isOnSuckUpPoint = false;
    public float speed = 5f; // 초기 이동 속도
    public float suckUpSpeed = 55f; // 흡입 속도

    GameObject UFO; // UFO 오브젝트
    Vector3 ufoPosition; // 목표 위치
    private AbductableObject abductableObject; // 애니메이션 컴포넌트

    bool suckedUp = false; // 흡입 여부
    bool hasPassedUFO = false; // UFO를 지나쳤는지 여부
    bool hasShownMiss = false; // Miss 판정을 이미 표시했는지 여부

    // 판정 관련 변수
    private float perfectRange = 0.2f; // Perfect 판정 범위
    private float goodRange = 0.5f; // Good 판정 범위

    void Start()
    {
        // UFO 오브젝트 찾기 (이름으로 찾기)
        UFO = GameObject.Find("UFO");
        if (UFO == null)
        {
            Debug.LogError("UFO 오브젝트를 찾을 수 없습니다.");
        }
        else
        {
            ufoPosition = UFO.transform.position; // UFO의 위치 저장
        }

        // AbductableObject 컴포넌트 추가
        abductableObject = gameObject.AddComponent<AbductableObject>();
    }

    void Update()
    {
        if (!suckedUp)
        {
            // 왼쪽으로 이동
            transform.position += Vector3.left * speed * Time.deltaTime;

            // UFO를 지나쳤는지 체크 (납치되지 않은 상태에서만)
            if (!hasPassedUFO && transform.position.x < ufoPosition.x)
            {
                hasPassedUFO = true;
            }
        }

        // 화면 왼쪽 밖(-20f)으로 나가면 파괴
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }

        if (isOnSuckUpPoint && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)))
        {
            Debug.Log("SuckUpPoint에 닿았고 클릭됨");
            suckedUp = true;
            // 판정 계산 및 표시
            ShowJudgment();
            // 애니메이션 시작
            abductableObject.StartAbduction(ufoPosition);
        }
    }

    // 판정 계산 및 표시
    private void ShowJudgment()
    {
        string judgment;
        Color color;

        if (suckedUp)
        {
            // 납치 성공 시 판정
            float distance = Mathf.Abs(transform.position.x - ufoPosition.x);
            if (distance <= perfectRange)
            {
                judgment = "Perfect!";
                color = new Color(1f, 0.92f, 0.016f); // 노란색
            }
            else
            {
                judgment = "Good!";
                color = new Color(0.2f, 0.8f, 0.2f); // 초록색
            }
        }
        else
        {
            // 납치 실패 시 Miss 판정
            judgment = "Miss";
            color = new Color(0.8f, 0.2f, 0.2f); // 빨간색
        }

        JudgmentManager.Instance.ShowJudgment(judgment, color);
    }

    // SuckPoint와 겹치기 시작할 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SuckUpPoint"))
        {
            Debug.Log("SuckUpPoint에 닿음");
            isOnSuckUpPoint = true;
        }
    }

    // SuckPoint에서 벗어날 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SuckUpPoint"))
        {
            Debug.Log("SuckUpPoint에서 벗어남");
            isOnSuckUpPoint = false;

            // UFO를 지나쳤고 아직 Miss 판정을 표시하지 않았다면
            if (hasPassedUFO && !hasShownMiss && !suckedUp)
            {
                ShowJudgment();
                hasShownMiss = true;
            }
        }
    }
}
