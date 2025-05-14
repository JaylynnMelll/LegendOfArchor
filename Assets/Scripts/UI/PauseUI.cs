using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 일시정지 UI제어
public class PauseUI : BaseUI
{
    [SerializeField] private Button resumeButton; // 계속하기 버튼
    [SerializeField] private Button mainMenuButton; // 메인메뉴 버튼

    [SerializeField] private GameObject skillIconPrefab; // 스킨 아이콘 프리팹

    [SerializeField] private Transform iconRoot; // 스킬 아이콘 들어갈 위치

    private List<GameObject> skillIconPool = new List<GameObject>();



    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
        // 각 버튼 클릭 이벤트 연결
        resumeButton.onClick.AddListener(OnClickResumeButton);
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
    }

    // 획득한 스킬 아이콘 보여주는 함수
    public void ShowAcquiredSkillIcons(List<RuntimeSkill> trackingList)
    {
        // 기존 오브젝트 비활성화
        foreach (var skillIcon in skillIconPool)
            skillIcon.SetActive(false);

        // 필요한 만큼 재사용 또는 새로 생성
        for (int i = 0; i < trackingList.Count; i++)
        {
            RuntimeSkill runtimeSkill = trackingList[i];
            GameObject skillIcon;

            // 풀에 있는 오브젝트 재사용
            if (i < skillIconPool.Count)
            {
                skillIcon = skillIconPool[i];
            }
            // 필요한 수보다 풀이 작을 경우, 생성
            else
            {
                skillIcon = Instantiate(skillIconPrefab, iconRoot);
                skillIconPool.Add(skillIcon);
            }

            // 아이콘 오브젝트 활성화
            skillIcon.SetActive(true);

            // 아이콘 이미지 설정
            var iconImage = skillIcon.GetComponent<Image>();
            if (iconImage != null)
                iconImage.sprite = runtimeSkill.skill.icon;

            // 스택 텍스트 설정 (2이상인 경우에만 표시)
            var text = skillIcon.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = (runtimeSkill.currentStacks > 1) ? $"X {runtimeSkill.currentStacks}" : "";
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
