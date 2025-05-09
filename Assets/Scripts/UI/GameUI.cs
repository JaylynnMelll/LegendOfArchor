using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 게임중 상태 UI제어
public class GameUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI waveText; // 웨이브 텍스트 삭제?예정
    [SerializeField] private TextMeshProUGUI levelText; // 플레이어 레벨 텍스트
    [SerializeField] private TextMeshProUGUI goldText; // 플레이어 골드 텍스트
    [SerializeField] private Slider expSlider; // 겸험치 게이지

    private void Start()
    {
        UpdateLevel(1); // 초기 레벨 : 1
        UpdateExpSlider(0); // 초기 경험치 게이지 0%
    }

    // 경험치 게이지 업데이트
    public void UpdateExpSlider(float percentage)
    {
        expSlider.value = percentage;
    }

    // 레벨 업데이트
    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
    }

    // 골드 업데이트
    public void UpdateGold(int amount)
    {
        goldText.text = amount.ToString();
    }

    // 웨이브 업데이트
    public void UpdateWaveText(int wave)
    {
        waveText.text = wave.ToString();
    }

    // UI 상태
    protected override UIState GetUIState()
    {
        return UIState.Game;
    }
}
