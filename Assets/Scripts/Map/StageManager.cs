using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 임시 싱글톤 처리
    public static StageManager instance;

    [Header("Connected Components")]
    [SerializeField] GameUI gameUI; 

    // 방들의 부모 역할, 방 풀링 담당, 플레이어 위치
    [SerializeField] private Transform mapParent;
    [SerializeField] private MapPoolManager mapPoolManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ProjectileManager projectileManager;

    // 포탈 프리팹
    [SerializeField] private Portal portal;

    // 현재 스테이지 번호, 현재 활성화 된 방, 스테이지 숫자
    [SerializeField] public int currentStage = 1;
    private GameObject currentRoom;

    // enemy 관리
    private EnemyManager enemyManager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // 초기화
    public void Init(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
        LoadRoom(currentStage);
        gameUI.UpdateStageNumber(currentStage);
    }

    // 방 생성 및 초기화
    public void LoadRoom(int stage)
    {
        // 플레이어 위치 초기화. 보스룸이냐 아니냐에 따라 변화
        if (currentStage % 5 == 0)
        {
            playerTransform.position = new Vector3(0, -6f, 0);
        }

        else
        {
            playerTransform.position = new Vector3(0, -4f, 0);
        }

        // 이전 방이 있었다면 풀에 반환
        if (currentRoom != null)
            mapPoolManager.ReturnRoom(currentRoom);

        // 이전 장애물이 있었다면 풀에 반환
        if(currentRoom != null)
            ObstacleManager.Instance.ClearObstacles();

        // 보스 방인지 판단(5의 배수)
        RoomType type = (stage % 5 == 0) ? RoomType.Boss : RoomType.Normal;
        bool isBoss = type == RoomType.Boss;

        // 풀에서 방 꺼냄 (없으면 생성)
        currentRoom = mapPoolManager.GetRoom(type);

        // 방 위치와 부모 설정
        currentRoom.transform.position = Vector3.zero;
        currentRoom.transform.SetParent(mapParent);

        // enemy 스폰을 위한 스폰 포인트 찾기
        List<Transform> spawnPoints = new();

        // obstacle 스폰을 위한 포인트 찾기
        List<Transform> obstacleSpawnPoints = new();

        Transform obstacleParent = currentRoom.transform.Find("Obstacles");

        Transform spawnPointsParent = currentRoom.transform.Find("SpawnPoints");

        if(spawnPointsParent != null)
        {
            foreach (Transform point in spawnPointsParent)
            {
                // 개별 스폰 포인트들 추가
                spawnPoints.Add(point);
            }

        }
        if (obstacleParent != null)
        {
            foreach (Transform point in obstacleParent)
            {
                // 개별 스폰 포인트들 추가
                obstacleSpawnPoints.Add(point);
            }
        }
        else
        {
            Debug.LogWarning("SpawnPoints 오브젝트를 찾을 수 없습니다.");
        }

        // 조금 기다렸다가 적을 생성
        StartCoroutine(DelayedSpawnEnemies(spawnPoints, isBoss, stage));

        // 장애물 호출
        ObstacleManager.Instance.SpawnObstacles(obstacleSpawnPoints);

        // 포탈 비활성화
        portal.gameObject.SetActive(false);

        // 포탈에 닿았을 때 호출할 함수
        portal.OnPlayerEnterPortal = NextStage;
    }

    // 적을 조금 기다렸다가 생성하는 코루틴
    private IEnumerator DelayedSpawnEnemies(List<Transform> spawnPoints, bool isBoss, int stage)
    {
        // 원하는 지연 시간
        yield return new WaitForSeconds(0.5f); 

        if (isBoss)
        {
            BossType bossType = (stage % 10 == 0) ? BossType.Necromancer : BossType.Slime;
            enemyManager.SpawnEnemy(spawnPoints, isBoss, stage, bossType);
        }
        else
        {
            enemyManager.SpawnEnemy(spawnPoints, isBoss, stage);
        }

        // 클리어 감시 시작
        StartCoroutine(CheckRoomClear());
    }

    // 1초마다 Enemy가 모두 사라졌는지 감시하는 코루틴
    private IEnumerator CheckRoomClear()
    {
        while(!enemyManager.IsAllEnemyCleared())
        {
            yield return new WaitForSeconds(1f);
        }

        // 클리어 후 포탈 활성화
        ActivatePortal();
    }


    // 클리어 시 포탈 활성화
    public void ActivatePortal()
    {
        // 보스룸이냐 아니냐에 따라 포탈 위치 변화
        if(currentStage % 5 == 0)
        {
            portal.transform.position = new Vector3(0, 8f, 0);
        }
        else
        {
            portal.transform.position = new Vector3(0, 3f, 0);
        }
        portal.gameObject.SetActive(true);
    }

    // 다음 스테이지로 이동
    private void NextStage()
    {
        StartCoroutine(LoadNextStageWithDelay());
    }

    // 포탈 진입 후 지연 시간
    private IEnumerator LoadNextStageWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        currentStage++;
        projectileManager.DestroyAllProjectile();
        LoadRoom(currentStage);
        gameUI.UpdateStageNumber(currentStage);
    }

    // 현재 스테이지 값 반환
    public int GetCurrentStage() => currentStage;
}
