using UnityEngine;
using System.Collections;
using System;

public class SpawnManager : MonoBehaviour
{
    public GameObject humanPrefab; // Human 프리팹
    public float spawnInterval = 1f; // 생성 간격 (기본값 2초)

    // ground Tag가 붙은 오브젝트 찾기
    private GameObject ground;

    private bool isSpawning = false;

    void Start()
    {
        // "ground" 태그가 붙은 오브젝트 찾기
        ground = GameObject.FindGameObjectWithTag("Ground");

        if (ground == null)
        {
            Debug.LogError("Ground object not found! Make sure it has the 'Ground' tag.");
            return;
        }
    }

    // GameStartEvent에서 호출할 메서드
    public void StartSpawning(object sender, EventArgs e)
    {
        Debug.Log("스폰 시작");
        isSpawning = true;
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        Debug.Log("SpawnHumans coroutine started");
        while (isSpawning)
        {
            SpawnObjectOnGround(); // Ground에 Human 생성

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObjectOnGround()
    {
        float groundY = ground.transform.position.y; // Ground의 Y 좌표
        float groundHeight = ground.transform.localScale.y; // Ground의 높이 (localScale.y 사용)

        float humanHeight = humanPrefab.GetComponent<SpriteRenderer>().bounds.size.y; // Human의 높이

        // Human의 피벗이 중앙이면, 바닥에 딱 맞추려면
        float spawnY = groundY + (groundHeight / 2) + (humanHeight / 2);

        Vector3 cameraPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));

        // 스폰 위치 결정 (x는 원하는 위치, y는 계산된 값)
        Vector3 spawnPos = new Vector3(cameraPos.x, spawnY, 0);

        Instantiate(humanPrefab, spawnPos, Quaternion.identity);
    }
}
