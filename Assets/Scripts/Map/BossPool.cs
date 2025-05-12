using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum BossType { Slime, Necromancer }

public class BossPool : MonoBehaviour
{
    [System.Serializable]
    public class BossPoolInfo
    {
        public BossType bossType;
        public GameObject prefab;

        // �������� splitCount���� ť�� ����
        public List<Queue<GameObject>> splitQueues = new List<Queue<GameObject>>();

        // ��ũ�θǼ�ó�� �п����� �ʴ� ���� �ϳ��� ť�� �ʿ�
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

                // ������ �����տ��� maxSplitCount ��������
                SlimeBossController slime = pool.prefab.GetComponent<SlimeBossController>();

                if (slime == null)
                {
                    Debug.LogError("������ �����տ� SlimeBossController�� �����ϴ�.");
                }

                int maxSplit = slime.maxSplitCount;

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

    public GameObject GetBoss(BossType type, int splitCount = 0, Vector3? spawnPos = null)
    {
        var pool = bossPools.Find(p => p.bossType == type);

        if (pool == null)
        {
            Debug.LogError($"{type} ������ ã�� �� �����ϴ�.");
            return null;
        }

        GameObject boss = null;

        // �������̶�� splitCount ť���� ������
        if (type == BossType.Slime)
        {
            if (splitCount < 0 || splitCount >= pool.splitQueues.Count)
            {
                Debug.LogWarning($"������ splitCount {splitCount}�� ��ȿ���� �ʽ��ϴ�. 0���� �ʱ�ȭ�մϴ�.");
                splitCount = 0;
            }

            var queue = pool.splitQueues[splitCount];
            // Ǯ�� ������ ���� �����ϰ�, ������ ������
            boss = (queue.Count > 0) ? queue.Dequeue() : Instantiate(pool.prefab);

        }
        // ��ũ�θǼ�
        else
        {
            boss = (pool.normalQueue.Count > 0) ? pool.normalQueue.Dequeue() : Instantiate(pool.prefab);
        }

        // ��ġ ������ �ִٸ� ����
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
            Debug.LogError($"BossPool {type}�� ã�� �� �����ϴ�.");
            return;
        }

        boss.SetActive(false);


        if (type == BossType.Slime)
            pool.splitQueues[splitCount].Enqueue(boss);
        else
            pool.normalQueue.Enqueue(boss);
    }
}

