using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 플레이어 스탯 관리
    public PlayerStats playerStats { get; private set; }
    public PlayerController player { get; private set; }
    private ResourceController _playerResourceController;

    [SerializeField] private int currentWaveIndex = 0;

    private EnemyManager enemyManager;
    private UIManager uiManager;

    public static bool isFirstLoading = true;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerController>();
        player.Init(this);
        playerStats = PlayerStats.Instance;

        uiManager = FindObjectOfType<UIManager>();
        uiManager.InitPlayerHPBar(player.transform);

        _playerResourceController = player.GetComponent<ResourceController>();
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP);
        _playerResourceController.AddHealthChangeEvent(uiManager.ChangePlayerHP);

        enemyManager = GetComponentInChildren<EnemyManager>();
        enemyManager.Init(this);
    }

    private void Start()
    {
        if (!isFirstLoading)
        {
            StartGame();
        }
        else
        {
            isFirstLoading = false;
        }
    }

    public void StartGame()
    {
        uiManager.SetPlayGame();
        StartNextWave();
    }

    void StartNextWave()
    {
        currentWaveIndex += 1;
        // uiManager.ChangeWave(currentWaveIndex);
        enemyManager.StartWave(1 + currentWaveIndex / 5);
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

    public void EndOfWave()
    {
        StartNextWave();
    }

    public void GameOver()
    {
        enemyManager.StopWave();
        uiManager.SetGameOver();
    }
}
