using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Skill UI에 붙여줄 스크립트.
/// Skill UI를 관리해준다.
/// </summary>
public class SkillUI : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private SkillDataBase skillDataBase;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private PlayerSkillHandler playerSkillHandler;

    [Header("Connecting UI Components")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private List<SkillButtonData> skillButtons = new List<SkillButtonData>();

    private void Start()
    {
        ShowRandomThreeSkills();
    }

    private void  PrintLevelText(int level)
    {
        if (levelText == null) return;
        
        levelText.text = level.ToString();
    }

    /// <summary>
    /// 선택된 3개의 랜덤 스킬을 UI에 보여주고 각각의 버튼에 
    /// 스킬 정보를 저장해주는 기능을 하는 메서드
    /// </summary>
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
}
