using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    public static PlayerCamera Instance;

    private Vector3 originalLocalPos;
    private Vector3 originalWorldPos;

    public float cameraSpeed = 5.0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        originalLocalPos = transform.localPosition;
        originalWorldPos = transform.position;
    }

    // 플레이어 이동은 Update이므로 겹치지 않게 뒤에 처리
    private void LateUpdate()
    {
        if (StageManager.instance.currentStage % 5 == 0)
        {
            // z축은 그대로 유지
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // 보간
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);

            // 매 프레임 원위치 갱신
            originalWorldPos = transform.position;
        }
        else
        {
            transform.position = new Vector3(0,0,-10);
        }

    }

    public void Shake(float intensity, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * intensity;
            transform.position = originalWorldPos + new Vector3(offset.x, offset.y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalWorldPos;
    }
}
