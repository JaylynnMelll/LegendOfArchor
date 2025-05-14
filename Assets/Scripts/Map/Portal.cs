using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Action OnPlayerEnterPortal;

    [SerializeField] private AudioClip portalSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾� �±׿��� ��Ż �۵�
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(portalSound!= null)
            {
                SoundManager.PlayClip(portalSound, 0.2f);
            }

            // ����� �ݹ��� ������ ���� �� ����(MapManager.NextStage)
            OnPlayerEnterPortal?.Invoke();
        }
    }
}
