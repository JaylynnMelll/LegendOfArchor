using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    Game,
    LevelUp,
    Pause,
    GameOver,
}

public class UIManager : MonoBehaviour
{
    GameUI gameUI;
    LevelUpUI levelUpUI;
    GameOverUI gameOverUI;
    PauseUI pauseUI;
    private UIState currentState;
    public HPBarPool hpBarPool;

    public GameObject damageTextPrefab; // TextMeshPro 프리팹
    public Transform damageTextRoot; // World Space Canvas의 Transform

    private HPBarUI playerHpBarUI;

    // 현재 UI 상태
    public UIState CurrentState => currentState;

    // 체력바 프리팹 연결용
    [SerializeField] private GameObject hpBarPrefab;
    // 체력바 프리팹이 붙을 위치
    [SerializeField] private Transform hpBarRoot;

    private void Awake()
    {
        gameUI = GetComponentInChildren<GameUI>(true);
        gameUI.Init(this);
        levelUpUI = GetComponentInChildren<LevelUpUI>(true);
        levelUpUI.Init(this);
        gameOverUI = GetComponentInChildren<GameOverUI>(true);
        gameOverUI.Init(this);
        pauseUI = GetComponentInChildren<PauseUI>(true);
        pauseUI.Init(this);

        ChangeState(UIState.Game);
    }


    public void CreatePlayerHPBar(Transform target, ResourceController resource)
    {
        if (playerHpBarUI != null) return;
        GameObject hpBar = Instantiate(hpBarPrefab, hpBarRoot);
        playerHpBarUI = hpBar.GetComponent<HPBarUI>();

        hpBar.GetComponent<FollowHPBar>().SetTarget(target);
        playerHpBarUI.SetFillColor(Color.green);
        playerHpBarUI.Init(resource);
    }


    public GameObject CreateEnemyHPBar(Transform target, ResourceController resource)
    {
        GameObject hpBar = hpBarPool.GetHPBar(hpBarPrefab);
        resource.SetHealth(resource.MaxHealth);
        hpBar.GetComponent<FollowHPBar>().SetTarget(target);
        hpBar.GetComponent<HPBarUI>().SetFillColor(Color.red);
        hpBar.GetComponent<HPBarUI>().Init(resource);

        return hpBar;
    }

    public void ReturnEnemyHPBar(GameObject hpBar)
    {
        hpBarPool.ReturnHPBar(hpBar);
    }
    public void DestroyPlayerHPBar()
    {
        if (playerHpBarUI != null)
        {
            Destroy(playerHpBarUI.gameObject); // 완전히 제거
            playerHpBarUI = null;
        }
    }
    public void ShowDamageText(Vector3 worldPos, int damage)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        GameObject textObj = Instantiate(damageTextPrefab, screenPos, Quaternion.identity, damageTextRoot);

        var damageText = textObj.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.Init(damage);
        }
    }

    // 게임중 상태로 전환
    public void SetPlayGame()
    {
        ChangeState(UIState.Game);
    }

    // 일시정지 상태로 전환
    public void SetGamePause()
    {
        ChangeState(UIState.Pause);
    }

    // 게임 오버 상태로 전환
    public void SetGameOver()
    {
        ChangeState(UIState.GameOver);
    }

    // 골드 갱신
    public void ChangePlayerGold(int gold)
    {
        gameUI.UpdateGold(gold);
    }

    public void PlayerLevelUp(int level)
    {
        levelUpUI.ShowLevelUpUI(level);
        ChangeState(UIState.LevelUp);
    }

    // 경험치 바, 레벨 갱신
    public void ChangePlayerExpAndLevel(float currentExp, float maxExp, int level)
    {
        gameUI.UpdateExpSlider(currentExp / maxExp);
        gameUI.UpdateLevel(level);
    }

    // UI 상태 전환
    public void ChangeState(UIState state)
    {
        currentState = state;
        gameUI.SetActive(currentState);
        levelUpUI.SetActive(currentState);
        pauseUI.SetActive(currentState);
        gameOverUI.SetActive(currentState);
    }
}