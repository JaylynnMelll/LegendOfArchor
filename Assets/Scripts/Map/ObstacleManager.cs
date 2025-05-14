using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    private Queue<GameObject> obstaclePool = new();

    private AnimationHandler anim;

    public void SpawnObstacles(List<Transform> spawnPoints)
    {
        foreach (var point in spawnPoints)
        {
            var obstacle = GetObstacle();
            obstacle.transform.position = point.position;
            obstacle.SetActive(true);

            // 초기화
            var resource = obstacle.GetComponent<Obstacle>();
            anim = obstacle.GetComponent<AnimationHandler>();

            if(resource != null)
            {
                resource.SetHealth(20);
            }
        }
    }

    // 풀에 있으면 받아오고, 없으면 생성
    private GameObject GetObstacle()
    {
        if(obstaclePool.Count > 0)
        {
            return obstaclePool.Dequeue();
        }

        return Instantiate(obstaclePrefab);
    }

    // 반환
    public void ReturnObstacle(GameObject obstacle)
    {
        obstacle.SetActive(false);
        obstaclePool.Enqueue(obstacle);
    }

    // 스테이지 넘어가며 장애물 제거
    public void ClearObstacles()
    {
        foreach(var obj in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            ReturnObstacle(obj);
        }
    }
}
