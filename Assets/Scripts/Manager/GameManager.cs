using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 플레이어 스탯 관리
    public PlayerStats playerStats { get; private set; } // Exp, Gold 등
    public PlayerController player { get; private set; } // 플레이어 제어
    private ResourceController _playerResourceController; // 플레이어 체력, 리소스 접근

    [SerializeField] private int currentWaveIndex = 0;

    private EnemyManager enemyManager;
    // private StageManager stageManager;
    private UIManager uiManager;

    // private EnemyPool enemyPool;

    public static bool isFirstLoading = true; // 첫 실행 여부

    private void Awake()
    {
        instance = this;

        // 플레이어 객체 초기화 및 연결
        player = FindObjectOfType<PlayerController>();
        player.Init(this); // GameManager 참조 넘김

        playerStats = PlayerStats.Instance; // 스탯 싱글톤 가져오기

        // UIManager 찾기
        uiManager = FindObjectOfType<UIManager>();
        _playerResourceController = player.GetComponent<ResourceController>();
    }

    private void Start()
    {
        CreatePlayerHPBar(); // 플레이어 체력바 생성
        if (!isFirstLoading)
        {
            StartGame();
        }
        else
        {
            isFirstLoading = false;
        }
        enemyManager = GetComponentInChildren<EnemyManager>();
        enemyManager.Init(this);
    }

    public void StartGame()
    {
        uiManager.SetPlayGame(); // UI 상태 Game으로 설정
        // stageManager.Init(enemyManager);
    }

    // 체력바 생성
    public void CreatePlayerHPBar()
    {
        uiManager.CreateHPBar(player.transform, _playerResourceController, Color.green);
    }

    // 적 체력바 생성
    public void CreateEnemyHPBar(Transform enemy, ResourceController resource)
    {
        uiManager.CreateHPBar(enemy, resource, Color.red);
    }
    public void ShowDamageText(Vector3 worldPos, int damage)
    {
        uiManager.ShowDamageText(worldPos, damage);
    }

    public bool IsGamePlaying()
    {
        return uiManager.CurrentState == UIState.Game;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        uiManager.SetGamePause();
    }

    public void ResumeGame()
    {
        uiManager.ChangeState(UIState.Game);
        Time.timeScale = 1;
    }

    public void ReturnToMainMenu()
    {
        uiManager.ChangeState(UIState.Home);
        Time.timeScale = 0;
        // 사운스 설정 추가 예정
    }

    // 경험치 및 레벨 UI 업데이트
    public void UpdateExp()
    {
        uiManager.ChangePlayerExpAndLevel(
            playerStats.Exp,
            playerStats.MaxExp,
            playerStats.Level
            );
    }

    // 골드 UI 업데이트
    public void UpdateGold()
    {
        uiManager.ChangePlayerGold(playerStats.Gold);
    }
    public void GameOver()
    {
        uiManager.SetGameOver(); // UI 상태 변경
        Time.timeScale = 0;
        uiManager.DestroyPlayerHPBar();
    }
}
