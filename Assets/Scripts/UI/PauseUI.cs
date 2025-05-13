using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 일시정지 UI제어
public class PauseUI : BaseUI
{
    [SerializeField] private Button resumeButton; // 계속하기 버튼
    [SerializeField] private Button mainMenuButton; // 메인메뉴 버튼

    [SerializeField] private GameObject skillIconPrefab; // 스킨 아이콘 프리팹

    [SerializeField] private Transform iconRoot; // 스킬 아이콘 들어갈 위치



    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
        // 각 버튼 클릭 이벤트 연결
        resumeButton.onClick.AddListener(OnClickResumeButton);
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    // 획득한 스킬 아이콘 보여주는 함수
    public void ShowAcquiredSkillIcons(List<Skill> acquiredSkills)
    {
        // 전달 받은 스킬 아이콘 리스트가지고 아이콘 생성
        foreach (Skill skill in acquiredSkills)
        {
            // 스킬 아이콘 프리팹을 복제하여 iconRoot 위치로 생성
            GameObject skillIcon = Instantiate(skillIconPrefab, iconRoot);
            // 스킬아이콘 가져오기
            Image image = skillIcon.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = skill.icon;
            }
        }
    }

    // 계속하기 버튼 클릭 시 호출되는 함수
    public void OnClickResumeButton()
    {
        GameManager.instance.ResumeGame();
    }

    // 메인메뉴 버튼 클릭 시 호출되는 함수
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
