using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Skill UI에 있는 3개의 버튼에 각각 할당되어
/// 랜덤으로 선택된 스킬을 UI에 보여주고, 각 스킬의 정보를 저장해주는 스크립트.
/// </summary>
public class SkillButtonData : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private PlayerSkillHandler playerSkillHandler;
    [SerializeField] private RangeWeaponHandler rangeWeaponHandler;     // Dynamically assigned
    [SerializeField] private Cooldown cooldown;


    [Header("UI Components")]
    public Button button;
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;
    public Skill assignedSkill;

    [Header("Skill Applying Events")]
    public UnityEvent ApplyingSkillToStats;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        rangeWeaponHandler = FindObjectOfType<RangeWeaponHandler>();
        Debug.Log("RangeWeaponHandler found and assigned to SkillButtonData.");

        if (rangeWeaponHandler != null)
        {
            // Prevention of multiple event calls on scene reload
            ApplyingSkillToStats.RemoveAllListeners();

            // ApplyingSKillToStats이벤트에 메서드 추가 (Dynamically assigned)
            ApplyingSkillToStats.AddListener(rangeWeaponHandler.ResetStats);
            ApplyingSkillToStats.AddListener(rangeWeaponHandler.ApplyFinalStats);
        }
    }

    /// <summary>
    /// SkillButtonData의 각 버튼에 스킬 정보를 저장해주는 메서드.
    /// </summary>
    /// <param name="skill"></param>
    public void SetSkillDataToButton (Skill skill)
    {
        skillIcon.sprite = skill.icon;
        skillNameText.text = skill.skillName;
        assignedSkill = skill;
    }

    /// <summary>
    /// 스킬을 고르는 버튼을 클릭했을 때 선택한 스킬을 
    /// Player에게 적용시켜주는 메서드.
    /// </summary>
    public void AddSkillOnClick()
    {
        // 스킬 버튼이 다시 활성화 되기까지의 쿨다운 시간을 체크
        if (cooldown.IsCoolingDown)
        {
#if UNITY_EDITOR
            Debug.Log("It's cooling down!");
#endif
            return;
        }

        playerSkillHandler.SkillAcquired(assignedSkill);
        ApplyingSkillsToStats();

        // 쿨다운 시작
        cooldown.StartCoolingDown();     
    }

    public void ApplyingSkillsToStats()
    {
        Debug.Log("Skills are Applied to Stats!");
        ApplyingSkillToStats?.Invoke();
    }
}
