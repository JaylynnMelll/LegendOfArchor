using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    Home,
    Game,
    Pause,
    GameOver,
}

public class UIManager : MonoBehaviour
{
    HomeUI homeUI;
    GameUI gameUI;
    GameOverUI gameOverUI;
    private UIState currentState;

    PauseUI pauseUI;
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
        homeUI = GetComponentInChildren<HomeUI>(true);
        homeUI.Init(this);
        gameUI = GetComponentInChildren<GameUI>(true);
        gameUI.Init(this);
        gameOverUI = GetComponentInChildren<GameOverUI>(true);
        gameOverUI.Init(this);

        ChangeState(UIState.Home);

        pauseUI = GetComponentInChildren<PauseUI>(true);
        pauseUI.Init(this);

        // 처음엔 홈 상태로
        ChangeState(UIState.Home);
    }


    public void CreateHPBar(Transform target, ResourceController resource, Color color)
    {
        GameObject hpBar = Instantiate(hpBarPrefab, Vector3.zero, Quaternion.identity, hpBarRoot);

        var follow = hpBar.GetComponent<FollowHPBar>();
        follow.SetTarget(target);

        var hpBarUI = hpBar.GetComponent<HPBarUI>();
        hpBarUI.SetFillColor(color);
        hpBarUI.Init(resource);

        if (color == Color.green)
            playerHpBarUI = hpBarUI;
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
        homeUI.SetActive(currentState);
        gameUI.SetActive(currentState);
        pauseUI.SetActive(currentState);
        gameOverUI.SetActive(currentState);
    }
}