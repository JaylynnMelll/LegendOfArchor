using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Action OnPlayerEnterPortal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 태그에만 포탈 작동
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 연결된 콜백을 실행해 다음 방 연결(MapManager.NextStage)
            OnPlayerEnterPortal?.Invoke();
        }
    }
}
