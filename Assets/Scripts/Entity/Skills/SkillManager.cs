using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Skill과 관련된 모든 기능들을 관리해주는 매니저 클래스.
/// </summary>
public class SkillManager : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private SkillDataBase skillDataBase;
    [SerializeField] private ResourceController resourceController;
    [SerializeField] private PlayerSkillHandler playerSkillHandler;

    /// <summary>
    /// skillDataBase에서 랜덤으로 선택된 스킬 데이터중 3개를 선택하여 저장하는 배열.
    /// </summary>
    [SerializeField] public Skill[] chooseSkill = new Skill[3];

    /// <summary>
    /// Choose 3 skills at random index from skilDataBase and add them to the chooseSkill array.
    /// </summary>
    public void AddToChooseSkillList()
    {
        // chooseSkill 배열을 초기화
        chooseSkill = new Skill[3];

        for (int i = 0; i < chooseSkill.Length; i++)
        {
            // 랜덤 스킬을 추가해줄 때 중복되는 스킬이 추가되지 않도록 체크
            while (true)
            {
                int randomIndex = Random.Range(0, skillDataBase.runTimeskillList.Count);
                if (chooseSkill[i] == null)
                {
                    if (!chooseSkill.Contains(skillDataBase.runTimeskillList[randomIndex]))
                    {
                        chooseSkill[i] = skillDataBase.runTimeskillList[randomIndex];
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

    }

    public void ClearChooseSkillList()
    {
        // temp : 플레이어가 스킬을 선택한 뒤 UI가 꺼질 때 작동
        for (int i = 0; i < chooseSkill.Length; i++)
        {
            chooseSkill[i] = null;
        }
    }
}
