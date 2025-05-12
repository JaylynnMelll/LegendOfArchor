using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 적 타입
public enum EnemyType { Normal, Boss }

public class EnemyPool : MonoBehaviour
{
    [Header("프리팹 목록")]
    [SerializeField] private GameObject[] normalEnemies;
    [SerializeField] private GameObject[] bossEnemies;

    private Dictionary<GameObject, Queue<GameObject>> enemyPools = new();

    // 적 꺼내기
    public GameObject GetEnemy(EnemyType type, Vector3 spawnPos)
    {
        GameObject[] sourceArray = (type == EnemyType.Normal) ? normalEnemies : bossEnemies;

        if (sourceArray.Length == 0)
        {
            Debug.LogWarning($"EnemyType {type}에 등록된 프리팹이 없습니다.");
            return null;
        }

        // 정해진 타입 내에서 랜덤으로 꺼낸다
        GameObject prefab = sourceArray[Random.Range(0, sourceArray.Length)];

        if (!enemyPools.ContainsKey(prefab))
            enemyPools[prefab] = new Queue<GameObject>();

        Queue<GameObject> pool = enemyPools[prefab];

        GameObject enemy;

        if (pool.Count > 0)
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
        foreach (var prefab in normalEnemies.Concat(bossEnemies))
        {
            if (enemyInstance.name.Contains(prefab.name))
                return prefab;
        }
        return null;
    }
}
