using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : BaseUI
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI TotalClearedStageNums;

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
        restartButton.onClick.AddListener(OnClickRestartButton);
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void OnClickMainMenuButton()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void SetTotalClearedStageNums(int stageNumber)
    {
        TotalClearedStageNums.text = $"{stageNumber:D2}";
    }

    protected override UIState GetUIState()
    {
        return UIState.GameOver;
    }
}