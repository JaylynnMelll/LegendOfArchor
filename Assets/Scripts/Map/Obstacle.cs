using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : ResourceController
{
    [SerializeField] private Sprite destroyEffect;

    protected override void Awake()
    {
        base.Awake();
        SetHealth(MaxHealth);
    }

    protected override void Died()
    {
        // �ı� ����Ʈ
        if(destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // ������Ʈ ��Ȱ��ȭ or ����
        gameObject.SetActive(false);
    }
}
