using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    public float cameraSpeed = 5.0f;

    // 플레이어 이동은 Update이므로 겹치지 않게 뒤에 처리
    private void LateUpdate()
    {
        if (StageManager.instance.currentStage % 5 == 0)
        {
            // z축은 그대로 유지
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // 보간
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(0,0,-10);
        }

    }
}
