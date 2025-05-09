using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI 상태 열거형
public enum UIState
{
    Home,
    Game,
    GameOver,
}

public class UIManager : MonoBehaviour
{
    // 각각의 UI 클래스 참조
    HomeUI homeUI;
    GameUI gameUI;
    GameOverUI gameOverUI;
    HPBarUI hpBarUI;

    // 현재 UI 상태
    private UIState currentState;

    // 체력바 프리팹이 붙을 위치
    private Transform hpBarRoot;

    // 체력바 프리팹 연결용
    [SerializeField] private GameObject hpBarPrefab;



    private void Awake()
    {
        // 각각의 UI를 하위 오브젝트에서 찾아 초기화
        homeUI = GetComponentInChildren<HomeUI>(true);
        homeUI.Init(this);
        gameUI = GetComponentInChildren<GameUI>(true);
        gameUI.Init(this);
        gameOverUI = GetComponentInChildren<GameOverUI>(true);
        gameOverUI.Init(this);

        // 체력바가 붙을 위치 찾기
        hpBarRoot = gameUI.transform.Find("HpBar");

        // 처음엔 홈 상태로
        ChangeState(UIState.Home);
    }

    // 플레이어 체력바 UI 생성, 초기화
    public void InitPlayerHPBar(Transform target)
    {
        // 체력바 프리팹 인스턴스화
        GameObject hpBar = Instantiate(hpBarPrefab, hpBarRoot);

        // 체력바 따라다니게 설정
        var follow = hpBar.GetComponent<FollowHPBar>();
        follow.SetTarget(target);

        // 체력바 색상 설정
        var hpBarUI = hpBar.GetComponent<HPBarUI>();
        hpBarUI.SetFillColor(Color.green);

        // 초기 체력 수치 설정        
        var stats = PlayerStats.Instance;
        hpBarUI.UpdateHP(stats.CurrentHP, stats.MaxHP);

        this.hpBarUI = hpBarUI;// UIManager에서 사용하기위해 사용
    }

    // 게임중 상태로 전환
    public void SetPlayGame()
    {
        ChangeState(UIState.Game);
    }

    // 게임 오버 상태로 전환
    public void SetGameOver()
    {
        ChangeState(UIState.GameOver);
    }

    // 삭제예정
    public void ChangeWave(int waveIndex)
    {
        gameUI.UpdateWaveText(waveIndex);
    }

    // 골드 갱신
    public void ChangePlayerGold(int gold)
    {
        gameUI.UpdateGold(gold);
    }

    // 플레이어 체력 갱신
    public void ChangePlayerHP(float currentHP, float maxHP)
    {
        hpBarUI.UpdateHP(currentHP, maxHP);
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
        gameOverUI.SetActive(currentState);
    }
}