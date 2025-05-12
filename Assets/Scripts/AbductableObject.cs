using UnityEngine;
using System.Collections;

public class AbductableObject : MonoBehaviour
{
  private Vector3 originalScale;
  private Vector3 originalPosition;
  private bool isBeingAbducted = false;
  private float abductionSpeed = 5f;
  private float stretchFactor = 3f;
  private float maxStretchHeight = 3f;

  void Start()
  {
    originalScale = transform.localScale;
    originalPosition = transform.position;
  }

  public void StartAbduction(Vector3 ufoPosition)
  {
    if (!isBeingAbducted)
    {
      isBeingAbducted = true;
      StartCoroutine(AbductionAnimation(ufoPosition));
    }
  }

  IEnumerator AbductionAnimation(Vector3 ufoPosition)
  {
    float elapsedTime = 0f;
    Vector3 startPosition = transform.position;
    Vector3 startScale = transform.localScale;

    // UFO 위치를 약간 위로 조정
    Vector3 targetPosition = ufoPosition + Vector3.up * 0.5f;

    while (elapsedTime < 1f)
    {
      elapsedTime += Time.deltaTime * abductionSpeed;
      float t = Mathf.Clamp01(elapsedTime);

      // 위치 이동
      transform.position = Vector3.Lerp(startPosition, targetPosition, t);

      // 스케일 조정 (위로 늘어나는 효과)
      float currentStretch = Mathf.Lerp(1f, stretchFactor, t);
      float currentHeight = Mathf.Lerp(0f, maxStretchHeight, t);

      transform.localScale = new Vector3(
          originalScale.x / currentStretch,
          originalScale.y * currentStretch,
          originalScale.z
      );

      // 투명도 조정
      SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
      if (spriteRenderer != null)
      {
        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(1f, 0.5f, t);
        spriteRenderer.color = color;
      }

      yield return null;
    }

    // 애니메이션 완료 후 오브젝트 제거
    Destroy(gameObject);
  }
}