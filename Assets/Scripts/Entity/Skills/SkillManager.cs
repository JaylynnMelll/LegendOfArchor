using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    [SerializeField] private Transform weaponPivot;
    private WeaponHandler weaponHandler;
    
    private WeaponCategory weaponCategory;

    /// <summary>
    /// skillDataBase에서 랜덤으로 선택된 스킬 데이터중 3개를 선택하여 저장하는 배열.
    /// </summary>
    [SerializeField] public Skill[] chooseSkill = new Skill[3];

    private void Start()
    {
        weaponHandler = weaponPivot.GetComponentInChildren<WeaponHandler>();
        weaponCategory = weaponHandler.weaponCategory;
    }

    /// <summary>
    /// Choose 3 skills at random index from skilDataBase and add them to the chooseSkill array.
    /// </summary>
    public void AddToChooseSkillList()
    {
        // chooseSkill 배열을 초기화
        chooseSkill = new Skill[3];

        for (int i = 0; i < chooseSkill.Length; i++)
        {
            int retryLimit = 100;       // 최대 재시도 횟수
            bool isSkillChosen = true; // 스킬이 선택되었는지 여부
            List<Skill> ChosenSkillList = ChooseSkillList();

            // 랜덤 스킬을 추가해줄 때 중복되는 스킬이 추가되지 않도록 체크해주는 while문
            while (retryLimit-- > 0)
            {
                int randomIndex = Random.Range(0, ChosenSkillList.Count);
                Skill randomSkill = ChosenSkillList[randomIndex];

                // chooseSkill 배열 내 중복된 스킬은 스킵
                if (chooseSkill.Contains(randomSkill))
                    continue; 

                // 아직 획득하지 않은 스킬인 경우 바로 추가
                if (!playerSkillHandler.acquiredSkills.Contains(randomSkill))
                {
                    chooseSkill[i] = randomSkill;
                    break;
                }

                // 이미 보유한 스킬인 경우, 추가 가능한지 현재 스택 확인
                RuntimeSkill stackTrack = playerSkillHandler.trackingList.FirstOrDefault(s => s.skill == randomSkill);

#if UNITY_EDITOR
                if (stackTrack == null)
                {
                    Debug.Log($"stackTrack is null for skill: {randomSkill.name}");
                }
#endif

                // 이미 최대 스택인 경우 스킵
                if (stackTrack != null && stackTrack.currentStacks >= randomSkill.maxStacks)
                    continue;

                // 중복 아니고, 스택 가능하면 추가
                chooseSkill[i] = randomSkill;
                isSkillChosen = true;
                break;
            }
#if UNITY_EDITOR
            if (!isSkillChosen)
            {
                Debug.LogWarning($"No valid skill found for slot {i} after 100 retries.");
            }
#endif
        }
    }

    private List<Skill> ChooseSkillList()
    {
        // 플레이어가 장착한 무기의 타입에 따른 스킬 리스트를 반환
        List<Skill> skillList = new List<Skill>();

        if (weaponHandler != null)
        {
            var CommonSkillList = skillDataBase.CommonSkillList;
            var RangedWeaponSkillList = skillDataBase.RangedWeaponSkillList;
            var MeleeWeaponSkillList = skillDataBase.MeleeWeaponSkillList;

            switch (weaponCategory)
            {
                case WeaponCategory.Melee:
                    skillList.AddRange(CommonSkillList);
                    skillList.AddRange(MeleeWeaponSkillList);
                    break;

                case WeaponCategory.Ranged:
                    skillList.AddRange(CommonSkillList);
                    skillList.AddRange(RangedWeaponSkillList);
                    break;

                default:
                    skillList.AddRange(CommonSkillList);
                    break;
            }
        }
        foreach (var skill in skillList)
        {
            if (skill == null)
            {
                Debug.LogWarning("Skill is null");
            }
            else
            {
                Debug.Log($"Skill name: {skill.skillName}");
            }
        }
        return skillList;
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
