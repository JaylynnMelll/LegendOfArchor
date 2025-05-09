using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void InitEnemy(EnemyManager enemyManager, Transform player);
}
