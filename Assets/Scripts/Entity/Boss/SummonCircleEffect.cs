using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonCircleEffect : MonoBehaviour
{
    [SerializeField] private float growTime = 0.5f;
    [SerializeField] private float maxScale = 1.0f;

    private Vector3 initialScale = Vector3.zero;
    private Vector3 targetScale;

    private void Start()
    {
        targetScale = Vector3.one * maxScale;
        transform.localScale = initialScale;
        StartCoroutine(ScaleUp());
    }

    private IEnumerator ScaleUp()
    {
        float elapsed = 0f;

        while (elapsed < growTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / growTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        // 일정 시간 후 자동 제거
        Destroy(gameObject, 0.5f);
    }
}
