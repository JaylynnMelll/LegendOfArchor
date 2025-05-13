using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 레벨업 시 나오는 스킬 선택 UI 제어
public class LevelUpUI : BaseUI
{
    // [Header("Connected Components")]
    [SerializeField] private SkillDataBase skillDataBase;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private PlayerSkillHandler playerSkillHandler;

    // [Header("Connecting UI Components")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private List<SkillButtonData> skillButtons = new List<SkillButtonData>();

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);
    }
    public void ShowLevelUpUI(int level)
    {
        foreach (SkillButtonData skillButton in skillButtons)
        {
            skillButton.Init();
            Debug.Log("SkillButtonData initialized.");
        }

        PrintLevelText(level);
        ShowRandomThreeSkills();
    }

    private void ShowRandomThreeSkills()
    {
        skillManager.AddToChooseSkillList();
        Skill[] skills = skillManager.chooseSkill;

        for (int i = 0; i < skills.Length; i++)
        {
            if (skillButtons[i] == null) continue;

            skillButtons[i].GetComponent<SkillButtonData>().SetSkillDataToButton(skills[i]);
        }
    }

    public void PrintLevelText(int level)
    {
        if (levelText == null) return;
        levelText.text = level.ToString();
    }

    // UI 상태
    protected override UIState GetUIState()
    {
        return UIState.LevelUp;
    }
}
