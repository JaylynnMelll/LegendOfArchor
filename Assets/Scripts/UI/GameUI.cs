using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : BaseUI
{
    [SerializeField] private Button pauseButton; // 정지 버튼
    [SerializeField] private TextMeshProUGUI levelText; // 플레이어 레벨 텍스트
    [SerializeField] private TextMeshProUGUI goldText; // 플레이어 골드 텍스트
    [SerializeField] private Slider expSlider; // 겸험치 게이지
    [SerializeField] private float expFillSpeed = 3f; // 경험치 게이지 채워지는 속도
    private float targetExp;

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
        pauseButton.onClick.AddListener(OnClickPauseButton);
    }
    private void Start()
    {
        UpdateLevel(1); // 초기 레벨 : 1
        UpdateExpSlider(0); // 초기 경험치 게이지 0%
    }

    private void Update()
    {
        // 현재 경험치와 목표 경험치 차이가 적으면 업데이트 중단
        if (Mathf.Abs(expSlider.value - targetExp) > 0.01f)
        {
            // 현재 경험치 값을 목표 경험치 값으로 서서히 채움
            expSlider.value = Mathf.Lerp(expSlider.value, targetExp, Time.deltaTime * expFillSpeed);
        }
        else
        {
            expSlider.value = targetExp;
        }
    }

    public void OnClickPauseButton()
    {
        GameManager.Instance.PauseGame();
    }

    // 경험치 게이지 업데이트
    public void UpdateExpSlider(float percentage)
    {
        targetExp = percentage;
    }

    // 레벨 업데이트
    public void UpdateLevel(int level)
    {
        levelText.text = $"{level}";
    }

    // 골드 업데이트
    public void UpdateGold(int amount)
    {
        goldText.text = $"{amount}";
    }

    protected override UIState GetUIState()
    {
        return UIState.Game;
    }
}
