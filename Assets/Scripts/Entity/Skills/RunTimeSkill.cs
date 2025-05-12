using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeSkill
{
    public Skill skill;
    public int currentStacks;

    public RuntimeSkill(Skill skill)
    {
        this.skill = skill;
        currentStacks = 0;
    }

    public bool AddStack()
    {
        if (currentStacks < skill.maxStacks)
        {
            currentStacks++;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetStacks() => currentStacks = 0;
}
