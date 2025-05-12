using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player { get; private set; }
    private ResourceController _playerResourceController;

    [SerializeField] private int currentWaveIndex = 0;

    private EnemyManager enemyManager;
    private StageManager stageManager;
    private UIManager uiManager;

    private EnemyPool enemyPool;

    public static bool isFirstLoading = true;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerController>();
        player.Init(this);

        uiManager = FindObjectOfType<UIManager>();

        _playerResourceController = player.GetComponent<ResourceController>();
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP);
        _playerResourceController.AddHealthChangeEvent(uiManager.ChangePlayerHP);

        enemyPool = FindObjectOfType<EnemyPool>();

        stageManager = FindObjectOfType<StageManager>();

        enemyManager = GetComponentInChildren<EnemyManager>();
        enemyManager.Init(this, enemyPool);
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
        stageManager.Init(enemyManager);
    }

    // 스테이지 정보와 적 스폰을 stageManager에 요청
    public void RequestStageLoad(int stageNumber, bool isBossRoom)
    {
        stageManager.LoadRoom(stageNumber);
    }

    public void GameOver()
    {
        uiManager.SetGameOver();
    }
}
