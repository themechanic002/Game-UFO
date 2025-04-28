using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed = 2f; // 배경 이동 속도
    public float resetPositionX = -19.2f; // 배경이 왼쪽으로 완전히 나갔을 때 x값
    public float startPositionX = 19.2f;  // 다시 오른쪽으로 보낼 x값

    void Update()
    {
        // 왼쪽으로 이동
        transform.position += Vector3.left * speed * Time.deltaTime;

        // 배경이 왼쪽 끝을 벗어나면 오른쪽으로 위치 재설정
        if (transform.position.x < resetPositionX)
        {
            Vector3 pos = transform.position;
            pos.x = startPositionX;
            transform.position = pos;
        }
    }
}
