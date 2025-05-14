using UnityEngine;

public enum WhoShotIt
{
    Player,
    Enemy
}
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask levelCollisionLayer;
    [SerializeField] private Skill bounceShot;
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
        Debug.Log("successfully grabbed playerSkillHandler");
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
        // 벽에 화살이 부딪힌 경우
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))
        {
            // 벽반사 스킬을 획득한 경우
            if (whoShotIt == WhoShotIt.Player && playerSkillHandler.acquiredSkills.Contains(bounceShot))
            {
                BounceShotApplied(collision);
            }
            else
            {
                DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestory);
            }
        }
        
        // 적과 화살이 부딪힌 경우
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
        {
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                float finalDamage = rangeWeaponHandler.DamageCalculator() * damageMultiplier;
                resourceController.ChangeHealth(-finalDamage);

                if (rangeWeaponHandler.IsOnKnockback)
                {
                    BaseController controller = collision.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        controller.ApplyKnockback(transform, rangeWeaponHandler.KnockbackPower, rangeWeaponHandler.KnockbackTime);
                    }
                }
            }

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

    private void BounceShotApplied(Collider2D collision)
    {
        // Get wall normal
        Vector2 wallNormal = (transform.position - (Vector3)collision.ClosestPoint(transform.position)).normalized;

        // Reflect current direction using wall normal
        direction = Vector2.Reflect(direction, wallNormal).normalized;

        damageMultiplier *= 0.5f;

        // Rotate to face new direction
        transform.right = direction;

        // 2번 반사 후 화살 파괴
        if (--bounceCount <= 0)
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
