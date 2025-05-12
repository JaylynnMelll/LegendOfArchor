using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class EnemyManager : MonoBehaviour
{
    private GameManager gameManager;
    private EnemyPool enemyPool;
    private BossPool bossPool;

    // 남아있는 적의 수
    public int aliveEnemyCount = 0;
    public void Init(GameManager gameManager, EnemyPool enemyPool, BossPool bossPool)
    {
        this.gameManager = gameManager;
        this.enemyPool = enemyPool;
        this.bossPool = bossPool;
    }
    // 스테이지가 진행될 때마다 호출되는 적 생성 메서드
    // 생성 위치, 보스 스테이지인지의 여부, 스테이지 번호를 받아온다
    public void SpawnEnemy(List<Transform> spawnPoints, bool isBossStage, int stageNumber, BossType? bossType = null)
    {
        // spawnPoint에 적 생성
        foreach (var spawnPoint in spawnPoints)
        {
            Vector3 spawnPos = spawnPoint.position;

            GameObject enemy;

            if(isBossStage)
            {
                if(bossType == null)
                {
                    Debug.LogError("보스 스테이지인데 BossType이 null입니다.");
                    continue;
                }

                int splitCount = 0;
                enemy = bossPool.GetBoss(bossType.Value, splitCount, spawnPos);

                // 보스 초기화
                SlimeBossController sbController = enemy.GetComponent<SlimeBossController>();
                NecromancerBossController nbController = enemy.GetComponent<NecromancerBossController>();

                if (sbController != null)
                {
                    sbController.InitEnemy(this, gameManager.player.transform);
                    sbController.Reset();
                }
                else if (nbController != null)
                {
                    nbController.InitEnemy(this, gameManager.player.transform);
                    nbController.Reset();
                }
            }

            else
            {
                // 일반 적 소환
                // 초기화
                enemy = enemyPool.GetEnemy(spawnPos);
                EnemyController enemyController = enemy.GetComponent<EnemyController>();

                if (enemyController != null)
                {
                    enemyController.Init(this, gameManager.player.transform);
                    enemyController.Reset();
                }
            }

            aliveEnemyCount++;
        }
    }
    // 사망한 적을 풀에 반환
    public void RemoveEnemyOnDeath(IEnemy enemy)
    {
        if (!enemy.IsSummoned)
            aliveEnemyCount--;

        if (enemy is SlimeBossController sb)
        {
            bossPool.ReturnBoss(BossType.Slime, sb.gameObject, sb.splitCount);
        }
        else if (enemy is NecromancerBossController nb)
        {
            bossPool.ReturnBoss(BossType.Necromancer, nb.gameObject, 0);
        }
        else
        {
            enemyPool.ReturnEnemy(enemy.gameObject);
        }
    }
    // aliveEnemyCount가 0 이하일 때 true를 반환한다.
    public bool IsAllEnemyCleared() => aliveEnemyCount <= 0;
}