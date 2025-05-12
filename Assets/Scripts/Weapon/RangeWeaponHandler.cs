using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class RangeWeaponHandler : WeaponHandler
{
    [Header("Connected Components")]
    private ProjectileManager projectileManager;
    private Skill multishot;

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

    [SerializeField] private int numberOfProjectilesPerShot;
    public int NumberofProjectilesPerShot { get; set; } = 1;

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

    public override void ResetStats()
    {
        WeaponSize = 0.7f;
        WeaponPower = 5;
        WeaponSpeed = 10f;
        WeaponRange = 10f;
        CriticalChance = 0.2f;
        CriticalChance = 1.5f;
        KnockbackPower = 0.1f;
        KnockbackTime = 0.5f;
        NumberofProjectilesPerShot = 1;

    }

    public override void ApplyFinalStats()
    {
        base.ApplyFinalStats();
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
