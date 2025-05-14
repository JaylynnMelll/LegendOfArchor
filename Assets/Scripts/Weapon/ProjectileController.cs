using UnityEngine;

public enum WhoShotIt
{
    Player,
    Enemy
}
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask levelCollisionLayer;
    [SerializeField] private Skill bouncingShot;
    [SerializeField] private Skill piercingShot;
    [SerializeField] private WhoShotIt whoShotIt;
    private PlayerSkillHandler playerSkillHandler;
    private RangeWeaponHandler rangeWeaponHandler;
    private ProjectileManager projectileManager;

    private float currentDuration;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer spriteRenderer;

    public bool fxOnDestory = true;

    [SerializeField] private int bounceCount = 3;
    [SerializeField] private float damageMultiplier = 1f;

    private void Awake()
    {
        playerSkillHandler = FindObjectOfType<PlayerSkillHandler>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        if (!isReady)
        {
            return;
        }

        currentDuration += Time.deltaTime;

        if (currentDuration > rangeWeaponHandler.BulletDuration)
        {
            DestroyProjectile(transform.position, false);
        }

        _rigidbody.velocity = direction * rangeWeaponHandler.WeaponSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // º®¿¡ È­»ìÀÌ ºÎµúÈù °æ¿ì
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            // º®¹Ý»ç ½ºÅ³À» È¹µæÇÑ °æ¿ì
            if (whoShotIt == WhoShotIt.Player && playerSkillHandler.acquiredSkills.Contains(bouncingShot))
            {
                BouncingShotApplied(collision);
            }
            else
            {
                DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestory);
            }
        }
        
        // Àû°ú È­»ìÀÌ ºÎµúÈù °æ¿ì
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
        {
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                if (whoShotIt == WhoShotIt.Player)
                {
                    float finalDamage = rangeWeaponHandler.DamageCalculator() * damageMultiplier;
                    resourceController.ChangeHealth(-finalDamage);
                }
                else
                {
                    resourceController.ChangeHealth(-rangeWeaponHandler.WeaponPower);
                }
                

                if (rangeWeaponHandler.IsOnKnockback)
                {
                    BaseController controller = collision.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        controller.ApplyKnockback(transform, rangeWeaponHandler.KnockbackPower, rangeWeaponHandler.KnockbackTime);
                    }
                }
            }
            // °üÅë¼¦ ½ºÅ³À» È¹µæÇÑ °æ¿ì
            if (whoShotIt == WhoShotIt.Player && playerSkillHandler.acquiredSkills.Contains(piercingShot))
                PiercingShotApplied(collision);
            else
                DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestory);
        }
    }


    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler, ProjectileManager projectileManager)
    {
        this.projectileManager = projectileManager;

        rangeWeaponHandler = weaponHandler;

        this.direction = direction;
        currentDuration = 0;
        transform.localScale = Vector3.one * weaponHandler.WeaponSize;
        spriteRenderer.color = weaponHandler.ProjectileColor;

        transform.right = this.direction;

        if (this.direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0, 0);
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0);

        isReady = true;
    }

    private void BouncingShotApplied(Collider2D collision)
    {
        // Get wall normal
        Vector2 wallNormal = (transform.position - (Vector3)collision.ClosestPoint(transform.position)).normalized;

        // Reflect current direction using wall normal
        direction = Vector2.Reflect(direction, wallNormal).normalized;

        damageMultiplier *= 0.5f;

        // Rotate to face new direction
        transform.right = direction;

        // 2¹ø ¹Ý»ç ÈÄ È­»ì ÆÄ±«
        if (--bounceCount <= 0)
        {
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestory);
        }
    }

    private void PiercingShotApplied(Collider2D collision)
    {
        // Reduce damage by 67% (i.e., keep 33%)
        damageMultiplier *= 0.33f;

        // if damage is too small, destroy the projectile
        float projectedDamage = rangeWeaponHandler.DamageCalculator() * damageMultiplier;
        if (projectedDamage <= 1f) // You can tweak this threshold
        {
            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestory);
        }
    }

    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx)
        {
            projectileManager.CreateImpactParticlesAtPostion(position, rangeWeaponHandler);
        }

        Destroy(this.gameObject);
    }
}
