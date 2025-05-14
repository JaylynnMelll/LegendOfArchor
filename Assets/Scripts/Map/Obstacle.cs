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
        // 파괴 이펙트
        if(destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // 오브젝트 비활성화 or 제거
        gameObject.SetActive(false);
    }
}
