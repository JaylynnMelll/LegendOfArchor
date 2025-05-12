using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    // �ӽ� �������̽� ����
    GameObject gameObject { get; set; }

    void InitEnemy(EnemyManager enemyManager, Transform player);

    bool IsSummoned { get; }

    GameObject ConnectedHPBar { get; set; }
}
