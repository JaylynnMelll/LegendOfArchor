using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillButtonData : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private PlayerSkillHandler playerSkillHandler;
    [SerializeField] private Cooldown cooldown;


    [Header("UI Components")]
    public Button button;
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;
    public Skill assignedSkill;

    

    public void SetSkillDataToButton (Skill skill)
    {
        skillIcon.sprite = skill.icon;
        skillNameText.text = skill.skillName;
        assignedSkill = skill;
    }

    public void AddSkillOnClick()
    {
        // if a button is pressed once
        // starts the timer for click delay
        // while the delay time is not up yet
        // the button can't be pressed again.
        if (cooldown.IsCoolingDown)
        {
            Debug.Log("It's cooling down!");
            return;
        }

        playerSkillHandler.SkillAcquired(assignedSkill);

        cooldown.StartCoolingDown();
        
    }
}
