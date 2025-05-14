using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skill Scritable Object�� Runtime�� ����� �� �ֵ��� GameObject�� �����Ͽ� ����� �� �ֵ��� ���ִ� Ŭ����.
/// </summary>
public class SkillDataBase : MonoBehaviour
{
    public List<Skill> CommonSkillList = new List<Skill>();
    public List<Skill> RangedWeaponSkillList = new List<Skill>();
    public List<Skill> MeleeWeaponSkillList = new List<Skill>();
}
