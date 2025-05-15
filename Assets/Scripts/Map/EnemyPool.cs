using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [Header("일반 몬스터 프리팹 목록")]
    [SerializeField] private GameObject[] normalEnemies;

    [Header("보스 프리팹 목록")]
    [SerializeField] private GameObject slimeBossPrefab;
    [SerializeField] private GameObject necromancerBossPrefab;

    // 적 관리 딕셔너리
    private Dictionary<GameObject, Queue<GameObject>> enemyPools = new();

    // 적 꺼내기
    public GameObject GetEnemy(Vector3 spawnPos)
    {
        if (normalEnemies.Length == 0)
        {
            Debug.LogWarning("일반 몬스터에 등록된 프리팹이 없습니다.");
            return null;
        }

        // 일반 몬스터 프리팹 중 랜덤 선택
        GameObject prefab = normalEnemies[Random.Range(0, normalEnemies.Length)];
        return GetPublicEnemy(prefab, spawnPos);
    }

    // 보스 꺼내기
    public GameObject GetBossEnemy(BossType bossType, Vector3 spawnPos)
    {
        GameObject bossPrefab = GetBossPrefab(bossType);
        if (bossPrefab == null)
        {
            Debug.LogError($"BossType {bossType}에 해당하는 프리팹이 없습니다.");
            return null;
        }

        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        boss.SetActive(true);
        return boss;
    }

    // BossType에 따라 프리팹 반환
    private GameObject GetBossPrefab(BossType bossType)
    {
        return bossType switch
        {
            BossType.Slime => slimeBossPrefab,
            BossType.Necromancer => necromancerBossPrefab,
            _ => null
        };
    }

    // 적을 풀에서 꺼내거나 새로 생성하는 공통 함수
    private GameObject GetPublicEnemy(GameObject prefab, Vector3 spawnPos)
    {
        if(!enemyPools.ContainsKey(prefab))
            enemyPools[prefab] = new Queue<GameObject>();

        Queue<GameObject> pool = enemyPools[prefab];
        GameObject enemy;

        if(pool.Count > 0)
        {
            enemy = pool.Dequeue();
        }
        else
        {
            enemy = Instantiate(prefab);
            enemy.transform.SetParent(transform);
        }

        enemy.transform.position = spawnPos;
        enemy.SetActive(true);
        return enemy;
    }


    // 적 반환
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemy.transform.SetParent(transform);

        GameObject prefab = FindMatchingPrefab(enemy);
        if (prefab != null)
        {
            enemyPools[prefab].Enqueue(enemy);
        }
    }

    // 이름 기반 프리팹 찾기
    private GameObject FindMatchingPrefab(GameObject enemyInstance)
    {
        foreach (var prefab in normalEnemies)
        {
            if (enemyInstance.name.Contains(prefab.name))
                return prefab;
        }
        return null;
    }
}
