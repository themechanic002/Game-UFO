using UnityEngine;
using System.Collections;
using System;

public class SpawnManager : MonoBehaviour
{
    public GameObject malePrefab;    // 남성 프리팹
    public GameObject femalePrefab;  // 여성 프리팹
    public GameObject dogPrefab;     // 개 프리팹
    public GameObject catPrefab;     // 고양이 프리팹

    [Header("스폰 확률 설정")]
    [Range(0, 1)] public float maleProbability = 0.3f;    // 남성 스폰 확률
    [Range(0, 1)] public float femaleProbability = 0.3f;  // 여성 스폰 확률
    [Range(0, 1)] public float dogProbability = 0.2f;     // 개 스폰 확률
    [Range(0, 1)] public float catProbability = 0.2f;     // 고양이 스폰 확률

    public float spawnInterval = 1f; // 생성 간격 (기본값 1초)

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

        // 확률의 합이 1이 되도록 정규화
        float totalProbability = maleProbability + femaleProbability + dogProbability + catProbability;
        if (totalProbability != 1f)
        {
            Debug.LogWarning("확률의 합이 1이 되도록 자동으로 조정됩니다.");
            maleProbability /= totalProbability;
            femaleProbability /= totalProbability;
            dogProbability /= totalProbability;
            catProbability /= totalProbability;
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
        float groundY = ground.transform.position.y;
        float groundHeight = ground.transform.localScale.y;

        // 스폰할 프리팹 선택
        GameObject prefabToSpawn = SelectRandomPrefab();
        float objectHeight = prefabToSpawn.GetComponent<SpriteRenderer>().bounds.size.y;

        float spawnY = groundY + (groundHeight / 2) + (objectHeight / 2);
        Vector3 cameraPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        Vector3 spawnPos = new Vector3(cameraPos.x, spawnY, 0);

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // AbductableObject 컴포넌트 추가
        if (spawnedObject.GetComponent<AbductableObject>() == null)
        {
            spawnedObject.AddComponent<AbductableObject>();
        }
    }

    private GameObject SelectRandomPrefab()
    {
        float random = UnityEngine.Random.value;
        float cumulativeProbability = 0f;

        // 남성 프리팹
        cumulativeProbability += maleProbability;
        if (random <= cumulativeProbability) return malePrefab;

        // 여성 프리팹
        cumulativeProbability += femaleProbability;
        if (random <= cumulativeProbability) return femalePrefab;

        // 개 프리팹
        cumulativeProbability += dogProbability;
        if (random <= cumulativeProbability) return dogPrefab;

        // 고양이 프리팹
        return catPrefab;
    }
}
