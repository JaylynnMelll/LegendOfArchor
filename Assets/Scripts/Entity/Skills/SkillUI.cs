using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    [SerializeField] private SkillDataBase skillDataBase;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private PlayerSkillHandler playerSkillHandler; 

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

    private void ShowRandomThreeSkills()
    {
        // 01) 랜덤한 스킬 3개를 ChooseSkill배열에 저장
        // 02) 각 스킬을 각 버튼에 할당하여 UI에 보여줌
        skillManager.AddToChooseSkillList();
        Skill[] skills = skillManager.chooseSkill;

        for (int i = 0; i < skills.Length; i++)
        {
            if (skillButtons[i] == null) continue;
            
            skillButtons[i].GetComponent<SkillButtonData>().SetSkillDataToButton(skills[i]);
        }
    }
}
