using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public UnityEvent ApplySkillToStats;

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Unity Lifecycle]
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        rangeWeaponHandler = FindObjectOfType<RangeWeaponHandler>();
        Debug.Log("RangeWeaponHandler found and assigned to SkillButtonData.");

        if (rangeWeaponHandler != null)
        {
            // Prevention of multiple event calls on scene reload
            ApplySkillToStats.RemoveAllListeners();

            // Subscribing methods to ApplyingSKillToStats() events (Dynamically assigned)
            ApplySkillToStats.AddListener(rangeWeaponHandler.ResetStats);
            ApplySkillToStats.AddListener(rangeWeaponHandler.ApplyFinalStats);
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Public Methods]
    public void SetSkillDataToButton(Skill skill)
    {
        skillIcon.sprite = skill.icon;
        skillNameText.text = skill.skillName;
        assignedSkill = skill;
    }

    public void AddSkillOnClick()
    {
        if (cooldown.IsCoolingDown)
        {
#if UNITY_EDITOR
            Debug.Log("It's cooling down!");
#endif
            return;
        }

        playerSkillHandler.SkillAcquired(assignedSkill);
        ApplyingSkillsToStats();
        GameManager.instance.SkillAdded();

        cooldown.StartCoolingDown();
    }

    public void ApplyingSkillsToStats()
    {
        Debug.Log("Skills are Applied to Stats!");
        ApplySkillToStats?.Invoke();
    }
}
