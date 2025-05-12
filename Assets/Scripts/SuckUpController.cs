using UnityEngine;

public class SuckUpController : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 모바일 화면 터치 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {
                // SuckPoint 오브젝트와 Human 오브젝트가 닿았을 때

            }
        }
    }
}
