using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // �̱���
    public static ObstacleManager Instance { get; private set; }


    [SerializeField] private GameObject obstaclePrefab;
    private Queue<GameObject> obstaclePool = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnObstacles(List<Transform> spawnPoints)
    {
        foreach (var point in spawnPoints)
        {
            var obstacle = GetObstacle();

            // �θ�� ����
            obstacle.transform.SetParent(this.transform);
            obstacle.transform.position = point.position;
            obstacle.SetActive(true);

            // �ʱ�ȭ
            var resource = obstacle.GetComponent<Obstacle>();

            if(resource != null)
            {
                // ü�� ����
                resource.SetHealth(20);
            }
        }
    }

    // Ǯ�� ������ �޾ƿ���, ������ ����
    private GameObject GetObstacle()
    {
        if(obstaclePool.Count > 0)
        {
            return obstaclePool.Dequeue();
        }

        return Instantiate(obstaclePrefab);
    }

    // ��ȯ
    public void ReturnObstacle(GameObject obstacle)
    {
        obstacle.SetActive(false);
        obstaclePool.Enqueue(obstacle);
    }

    // �������� �Ѿ�� ��ֹ� ����
    public void ClearObstacles()
    {
        foreach(var obj in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            ReturnObstacle(obj);
        }
    }
}
