using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject humanPrefab; // Human 프리팹
    public float spawnInterval = 1f; // 생성 간격 (기본값 2초)

    private float spawnTimer = 0f;
    private float speed = 5f; // 초기 Human 속도
    private float speedTimer = 0f; // 속도 증가 타이머

    void Update()
    {
        // Human 생성 타이머
        spawnTimer += Time.fixedDeltaTime;
        if (spawnTimer >= spawnInterval)
        {
            GameObject human = Instantiate(humanPrefab, transform.position, Quaternion.identity);
            human.GetComponent<ObjectController>().speed = speed; // 현재 속도 적용
            spawnTimer = 0f;
        }

        // 5초마다 속도 2씩 증가
        speedTimer += Time.fixedDeltaTime;
        if (speedTimer >= 5f)
        {
            speed += 2f;
            speedTimer = 0f;
        }
    }
}
