using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private bool isOnSuckUpPoint = false;
    public float speed = 5f; // 초기 이동 속도
    public float suckUpSpeed = 55f; // 흡입 속도

    GameObject UFO; // UFO 오브젝트
    Vector3 ufoPosition; // 목표 위치

    bool suckedUp = false; // 흡입 여부

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
    }

    void Update()
    {
        if (suckedUp)
        {
            // UFO 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, ufoPosition, suckUpSpeed * Time.deltaTime);
        }
        else
        {
            // 왼쪽으로 이동
            transform.position += Vector3.left * speed * Time.deltaTime;
        }

        // 화면 왼쪽 밖(-20f)으로 나가면 파괴
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
        else if (transform.position == ufoPosition)
        {
            Destroy(gameObject);
        }

        if (isOnSuckUpPoint && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)))
        {
            Debug.Log("SuckUpPoint에 닿았고 클릭됨");
            suckedUp = true;
        }
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
        }
    }
}
