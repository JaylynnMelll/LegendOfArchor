using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum BossType { Slime, Necromancer }

public class BossPool : MonoBehaviour
{
    // 보스 타입별로 풀링 정보를 담음
    [System.Serializable]
    public class BossPoolInfo
    {
        public BossType bossType;
        public GameObject prefab;


        // 슬라임은 splitCount별로 큐를 나눔
        // 예시
        //splitQueues[0]: 큰 슬라임 보스
        //splitQueues[1]: 중간 슬라임 보스
        //splitQueues[2]: 작은 슬라임 보스
        public List<Queue<GameObject>> splitQueues = new List<Queue<GameObject>>();

        // 네크로맨서처럼 분열하지 않는 경우는 하나의 큐만 필요
        public Queue<GameObject> normalQueue = new Queue<GameObject>();

    }

    public List<BossPoolInfo> bossPools;


    private void Awake()
    {
        foreach (var pool in bossPools)
        {
            if (pool.bossType == BossType.Slime)
            {
                pool.splitQueues.Clear();

                // 슬라임 프리팹에서 maxSplitCount 가져오기
                SlimeBossController slime = pool.prefab.GetComponent<SlimeBossController>();

                if (slime == null)
                {
                    Debug.LogError("슬라임 프리팹에 SlimeBossController가 없습니다.");
                }

                //int maxSplit = slime.maxSplitCount;
                int maxSplit = 5;

                // maxSplit = 4일 경우 splitCount 0, 1, 2, 3, 4를 위한 큐 생성 
                for (int i = 0; i <= maxSplit; i++)
                {
                    pool.splitQueues.Add(new Queue<GameObject>());
                }
            }
            else
            {
                pool.normalQueue = new Queue<GameObject>();
            }
        }
    }

    public GameObject GetBoss(BossType type, int splitCount, Vector3? spawnPos = null)
    {
        // bossPool 리스트에서 원하는 type을 pool 변수에 담는 과정
        var pool = bossPools.Find(p => p.bossType == type);

        if (pool == null)
        {
            Debug.LogError($"{type} 보스를 찾을 수 없습니다.");
            return null;
        }

        GameObject boss = null;

        // 슬라임이라면 splitCount 큐에서 가져옴
        if (type == BossType.Slime)
        {
            if (splitCount < 0 || splitCount >= pool.splitQueues.Count)
            {
                Debug.LogWarning($"슬라임 splitCount {splitCount}가 유효하지 않습니다. 0으로 초기화합니다.");
                splitCount = 0;
            }

            var queue = pool.splitQueues[splitCount];
            // 풀에 없으면 새로 생성하고, 있으면 꺼낸다
            boss = (queue.Count > 0) ? queue.Dequeue() : Instantiate(pool.prefab, this.transform);

        }
        // 네크로맨서
        else
        {
            boss = (pool.normalQueue.Count > 0) ? pool.normalQueue.Dequeue() : Instantiate(pool.prefab, this.transform);
        }

        // 위치 설정이 있다면 적용
        if (spawnPos.HasValue)
        {
            boss.transform.position = spawnPos.Value;
            boss.transform.rotation = Quaternion.identity;
        }

        boss.SetActive(true);
        return boss;
    }

    public void ReturnBoss(BossType type, GameObject boss, int splitCount)
    {
        var pool = bossPools.Find(p => p.bossType == type);

        if (pool == null)
        {
            Debug.LogError($"BossPool {type}을 찾을 수 없습니다.");
            return;
        }

        boss.SetActive(false);
        boss.transform.SetParent(this.transform);


        if (type == BossType.Slime)
            pool.splitQueues[splitCount].Enqueue(boss);
        else
            pool.normalQueue.Enqueue(boss);
    }
}

