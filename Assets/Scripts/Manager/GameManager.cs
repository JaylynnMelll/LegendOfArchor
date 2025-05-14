using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 플레이어 스탯 관리
    public PlayerStats playerStats { get; private set; } // Exp, Gold 등
    public PlayerController player { get; private set; } // 플레이어 제어
    private ResourceController _playerResourceController; // 플레이어 체력, 리소스 접근

    [SerializeField] private int currentWaveIndex = 0;

    private EnemyManager enemyManager;
    private StageManager stageManager;
    private UIManager uiManager;
    private EnemyPool enemyPool;
    private BossPool bossPool;

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

        // Pool 찾기
        enemyPool = FindObjectOfType<EnemyPool>();
        bossPool = FindObjectOfType<BossPool>();

        enemyManager = GetComponentInChildren<EnemyManager>();
        enemyManager.Init(this, enemyPool, bossPool);
        stageManager = FindObjectOfType<StageManager>();
    }

    private void Start()
    {
        CreatePlayerHPBar(); // 플레이어 체력바 생성
        stageManager.Init(enemyManager);

        // ✅ 선택된 무기 프리팹 이름 불러와 장착
        string prefabName = PlayerPrefs.GetString("SelectedWeaponPrefabName", "P_Golden_sword_EquipWeapon");
        EquipWeapon(prefabName);

        if (isFirstLoading)
            isFirstLoading = false;
    }

    // ✅ 무기 장착 함수 (무기 교체용)
    public void EquipWeapon(string prefabName)
    {
        Transform pivot = player.transform.Find("WeaponPivot");

        // 기존 무기 제거
        foreach (Transform child in pivot)
            Destroy(child.gameObject);

        // 프리팹 로드 및 장착
        GameObject prefab = Resources.Load<GameObject>($"Weapons/{prefabName}");
        if (prefab != null)
        {
            GameObject weapon = Instantiate(prefab, pivot);
            WeaponHandler handler = weapon.GetComponent<WeaponHandler>();

            PlayerSkillHandler skillHandler = player.GetComponent<PlayerSkillHandler>();
            handler.SetAsPlayerWeapon(skillHandler);
            player.SetWeaponHandler(handler);
        }
        else
        {
            Debug.LogWarning($"무기 프리팹 로드 실패: {prefabName}");
        }
    }

    // 체력바 생성
    public void CreatePlayerHPBar()
    {
        uiManager.CreatePlayerHPBar(player.transform, _playerResourceController);
    }

    // 적 체력바 생성
    public GameObject CreateEnemyHPBar(Transform enemyTransform, ResourceController resource)
    {
        return uiManager.CreateEnemyHPBar(enemyTransform, resource);
    }

    public void ReturnEnemyHPBar(GameObject hpBar)
    {
        uiManager.ReturnEnemyHPBar(hpBar);
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
        Time.timeScale = 1;
        uiManager.ChangeState(UIState.Game);
    }

    public void SkillAdded()
    {
        Time.timeScale = 1;
        uiManager.ChangeState(UIState.Game);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScene");
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

    public void LevelUp(int level)
    {
        Time.timeScale = 0;
        uiManager.PlayerLevelUp(level);
    }

    // 스테이지 정보와 적 스폰을 stageManager에 요청
    public void RequestStageLoad(int stageNumber, bool isBossRoom)
    {
        stageManager.LoadRoom(stageNumber);
    }

    // 게임 오버 처리
    public void GameOver()
    {
        uiManager.SetGameOver();
        Time.timeScale = 0;
        uiManager.DestroyPlayerHPBar();
    }
}