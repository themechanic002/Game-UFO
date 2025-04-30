using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public float speed = 5f; // 초기 이동 속도

    void Update()
    {
        // 왼쪽으로 이동
        transform.position += Vector3.left * speed * Time.deltaTime;

        // 화면 왼쪽 밖(-20f)으로 나가면 파괴
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}
