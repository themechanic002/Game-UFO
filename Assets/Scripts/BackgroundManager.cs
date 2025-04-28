using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject backgroundPrefab; // 배경 프리팹
    public float speed = 2f;            // 배경 이동 속도
    public int backgroundCount = 2;     // 이어붙일 배경 개수

    private GameObject[] backgrounds;
    private float bgWidth;

    void Start()
    {
        // 배경 프리팹의 가로 길이 계산
        bgWidth = backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        // 배경 오브젝트 배열 생성
        backgrounds = new GameObject[backgroundCount];

        // 배경 오브젝트를 이어붙여 생성
        for (int i = 0; i < backgroundCount; i++)
        {
            Vector3 pos = new Vector3(i * bgWidth, 0, 0);
            backgrounds[i] = Instantiate(backgroundPrefab, pos, Quaternion.identity, transform);

            // SpriteRenderer 컴포넌트 가져오기
            SpriteRenderer sr = backgrounds[i].GetComponent<SpriteRenderer>();

            // Sorting Layer와 Order in Layer 설정
            sr.sortingLayerName = "Background"; // 원하는 Sorting Layer 이름
            sr.sortingOrder = 0;                // 원하는 Order in Layer 값
        }
    }

    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // 왼쪽으로 이동
            backgrounds[i].transform.position += Vector3.left * speed * Time.deltaTime;
        }

        // 첫 번째 배경이 화면 밖으로 나가면 맨 뒤로 이동
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].transform.position.x < -bgWidth)
            {
                // 맨 뒤 배경의 x 위치 계산
                float rightMostX = backgrounds[0].transform.position.x;
                for (int j = 1; j < backgrounds.Length; j++)
                {
                    if (backgrounds[j].transform.position.x > rightMostX)
                        rightMostX = backgrounds[j].transform.position.x;
                }
                // 현재 배경을 맨 뒤로 이동
                backgrounds[i].transform.position = new Vector3(rightMostX + bgWidth, 0, 0);
            }
        }
    }
}
