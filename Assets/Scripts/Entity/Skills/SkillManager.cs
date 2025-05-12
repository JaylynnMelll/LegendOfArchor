using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SkillDataBase skillDataBase;

    [SerializeField] public Skill[] chooseSkill = new Skill[3];

    private void Awake()
    {
        skillDataBase = GetComponent<SkillDataBase>();
    }

    public void AddToChooseSkillList()
    {
        // skilldatabase의 스킬리스트에서 랜덤한 인덱스의 스킬을 3개 선택하여 chooseSkill에 추가
        for (int i = 0; i < chooseSkill.Length; i++)
        {
            // 랜덤 스킬을 추가해줄 때 중복되는 스킬이 추가되지 않도록 체크
            while (true)
            {
                int randomIndex = Random.Range(0, skillDataBase.skillList.Count);
                if (chooseSkill[i] == null)
                {
                    if (!chooseSkill.Contains(skillDataBase.skillList[randomIndex]))
                    {
                        chooseSkill[i] = skillDataBase.skillList[randomIndex];
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
}
