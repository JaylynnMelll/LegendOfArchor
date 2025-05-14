using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class RangeWeaponHandler : WeaponHandler
{
    [Header("Connected Components")]
    private ProjectileManager projectileManager;

    [Header("Ranged Attack Info")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int bulletIndex;
    public int BulletIndex { get { return bulletIndex; } }

    [SerializeField] private float bulletSize = 1f;
    public float BulletSize { get { return bulletSize; } }

    [SerializeField] private float bulletDuration;
    public float BulletDuration { get { return bulletDuration; } }

    [SerializeField] private float bulletSpread;
    public float Spread { get { return bulletSpread; } }

    [SerializeField] private int numberOfProjectilesPerShot = 1;
    public int NumberofProjectilesPerShot
    {
        get { return numberOfProjectilesPerShot; }
        set
        {
            numberOfProjectilesPerShot = value;
            if (numberOfProjectilesPerShot < 1)
            {
                numberOfProjectilesPerShot = 1;
            }
        }
    }

    [SerializeField] private float multipleProjectileAngle;
    public float MultipleProjectileAngle { get { return multipleProjectileAngle; } }

    [SerializeField] private Color projectileColor;
    public Color ProjectileColor { get { return projectileColor; } }

    protected override void Start()
    {
        base.Start();
        projectileManager = ProjectileManager.Instance;
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Public Methods]
    public override void Attack()
    {
        base.Attack();

        // 기본적인 공격
        foreach (float angle in GetArrowAngle())
        {
            CreateProjectile(Controller.LookDirection, angle);
        }
    }

    public void ProjectileManagerNullCheck()
    {
        if (projectileManager != null)
        {
            projectileManager = ProjectileManager.Instance;
        }
    }

    public void CreateProjectile(Vector2 _lookDirection, float angle)
    {
        projectileManager.ShootBullet(
            this,
            projectileSpawnPosition.position,
            RotateVector2(_lookDirection, angle)
            );
    }

    public override void ResetWeaponStats()
    {
        base.ResetWeaponStats();
    }

    public override void ApplyFinalWeaponStats()
    {
        base.ApplyFinalWeaponStats();
    }

    public float DamageCalculator()
    {
        // return the critical damage or normal damage
        float critChance = Random.Range(0f, 1f);

        if (critChance <= CriticalChance)
        {
            return WeaponPower * CriticalDamage;
        }
        else
        {
            return WeaponPower;
        }
    }

    public void MultiShot()
    {
        if (playerSkillHandler.trackingList != null)
        {
            RuntimeSkill multishotSkill = playerSkillHandler.trackingList.Find(s => s.skill.skillID == multiShotSkill.skillID);

            if (multishotSkill == null) return;

            if (multishotSkill.currentStacks <= multiShotSkill.maxStacks)
            {
                NumberofProjectilesPerShot += 1;
            }
            else
            {
                return;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // [Private Methods]
    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;
    }

    private List<float> GetArrowAngle()
    {
        // 각각의 화살 사이 각도를 계산하는 메서드
        List<float> angles = new List<float>();

        float projectileAngleSpace = multipleProjectileAngle;
        int numberOfProjectilePerShot = numberOfProjectilesPerShot;

        float startAngle = -(numberOfProjectilesPerShot / 2f) * projectileAngleSpace;

        for (int i = 0; i < numberOfProjectilesPerShot; i++)
        {
            float angle = startAngle + (i * projectileAngleSpace);
            float randomSpread = Random.Range(-Spread, Spread);
            angle += randomSpread;

            angles.Add(angle);
        }

        return angles;
    }
}
