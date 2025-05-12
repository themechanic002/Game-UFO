using UnityEngine;

public class MaskController : MonoBehaviour
{
  private SpriteMask spriteMask;
  private GameObject ground;
  private GameObject ufo;
  private Camera mainCamera;

  void Start()
  {
    // 필요한 컴포넌트와 오브젝트 찾기
    spriteMask = GetComponent<SpriteMask>();
    ground = GameObject.FindGameObjectWithTag("Ground");
    ufo = GameObject.Find("UFO");
    mainCamera = Camera.main;

    if (ground == null || ufo == null || mainCamera == null)
    {
      Debug.LogError("필요한 오브젝트를 찾을 수 없습니다!");
      return;
    }

    // 마스크 크기와 위치 설정
    UpdateMaskSize();
  }

  void UpdateMaskSize()
  {
    // 화면의 좌우 크기 계산
    float screenWidth = mainCamera.orthographicSize * 2 * mainCamera.aspect;

    // Ground와 UFO 사이의 높이 계산
    float groundY = ground.transform.position.y;
    float ufoY = ufo.transform.position.y;
    float maskHeight = Mathf.Abs(ufoY - groundY);

    // 마스크 스프라이트 크기 설정
    spriteMask.transform.localScale = new Vector3(screenWidth, maskHeight, 1);

    // 마스크 위치 설정 (Ground와 UFO의 중간 지점)
    float maskY = (groundY + ufoY) / 2;
    spriteMask.transform.position = new Vector3(0, maskY, 0);
  }
}