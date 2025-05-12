using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 일시정지 상태 UI제어
public class PauseUI : BaseUI
{
    [SerializeField] private Button resumeButton; // 계속하기 버튼
    [SerializeField] private Button mainMenuButton; // 메인메뉴 버튼


    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
        resumeButton.onClick.AddListener(OnClickResumeButton);
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    public void OnClickResumeButton()
    {
        GameManager.instance.ResumeGame();
    }

    public void OnClickMainMenuButton()
    {
        GameManager.instance.ReturnToMainMenu();
    }

    // UI 상태
    protected override UIState GetUIState()
    {
        return UIState.Pause;
    }
}
