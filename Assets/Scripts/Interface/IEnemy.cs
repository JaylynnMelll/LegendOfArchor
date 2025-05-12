using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    // 임시 인터페이스 구현
    GameObject gameObject { get; set; }

    void InitEnemy(EnemyManager enemyManager, Transform player);
}
