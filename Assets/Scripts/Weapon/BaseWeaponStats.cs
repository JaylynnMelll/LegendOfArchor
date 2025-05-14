using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponStats : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] private float attackDelay;
    public float AttackDelay { get => attackDelay; set => attackDelay = value; }

    [SerializeField] private float weaponSize;
    public float WeaponSize { get => weaponSize; set => weaponSize = value; }

    [SerializeField] private float weaponPower;
    public float WeaponPower { get => weaponPower; set => weaponPower = value; }

    [SerializeField] private float weaponSpeed;
    public float WeaponSpeed { get => weaponSpeed; set => weaponSpeed = value; }

    [SerializeField] private float weaponRange;
    public float WeaponRange { get => weaponRange; set => weaponRange = value; }

    [SerializeField] private float ciriticalChance;
    public float CriticalChance { get => ciriticalChance; set => ciriticalChance = value; }

    [SerializeField] private float ciriticalDamage;
    public float CriticalDamage { get => ciriticalDamage; set => ciriticalDamage = value; }

    [Header("KnockBack Info")]
    [SerializeField] private float knockbackPower;
    public float KnockbackPower { get => knockbackPower; set => knockbackPower = value; }

    [SerializeField] private float knockbackTime;
    public float KnockbackTime { get => knockbackTime; set => knockbackTime = value; }
}

