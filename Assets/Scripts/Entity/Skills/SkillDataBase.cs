using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skill Scritable Object를 Runtime에 사용할 수 있도록 GameObject에 부착하여 사용할 수 있도록 해주는 클래스.
/// </summary>
public class SkillDataBase : MonoBehaviour
{
    public List<Skill> CommonSkillList = new List<Skill>();
    public List<Skill> RangedWeaponSkillList = new List<Skill>();
    public List<Skill> MeleeWeaponSkillList = new List<Skill>();
}
