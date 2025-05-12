using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }

    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.5f;

    protected AnimationHandler animationHandler;
    protected StatHandler statHandler;

    [SerializeField] public WeaponHandler WeaponPrefab;
    protected WeaponHandler weaponHandler;

    protected bool isAttacking;
    private float timeSinceLastAttack = 0.0f;

    [SerializeField] private Material dieMaterial;
    [SerializeField] private Material resetMaterial;

    ResourceController resourceController;

    // 리셋 작업을 위한 초기 상태 저장
    private Vector2 initialMovementDirection = Vector2.zero;  // 초기 이동 방향
    private Vector2 initialLookDirection = Vector2.right;     // 초기 시선 방향
    private bool initialIsAttacking = false;                   // 초기 공격 상태
    private float initialHealth;                               // 초기 체력
    private float initialKnockbackDuration = 0.5f;               // 초기 넉백 시간

    // -------------------------------------------------------------------------------------------------------------------------------------------------------------

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
        statHandler = GetComponent<StatHandler>();
        resourceController = GetComponent<ResourceController>();

        Initialize();

        if (WeaponPrefab != null)
        {
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);
        }
        else
        {
            weaponHandler = GetComponentInChildren<WeaponHandler>();
        }
    }

    // 적이 태어날 때 호출 (혹은 처음 초기화되는 곳에서 설정)
    public void Initialize()
    {
        // 초기 상태 값 설정
        initialMovementDirection = Vector2.zero;  // 초기 방향은 0,0 (움직이지 않음)
        initialLookDirection = Vector2.right;     // 기본적으로 오른쪽을 향함
        initialIsAttacking = false;               // 초기에는 공격하지 않음
        initialHealth = statHandler.Health;       // 초기 체력은 현재 체력으로 설정
        initialKnockbackDuration = 0f;            // 넉백 상태 없음
    }

    protected virtual void Start()
    {
       
    }

    protected virtual void Update()
    {
        HandleAction();
        Rotate(lookDirection);
        HandleAttackDelay();
    }

    protected virtual void FixedUpdate()
    {
        Movement(movementDirection);
        if (knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleAction()
    {

    }

    private void Movement(Vector2 direction)
    {
        direction = direction * statHandler.Speed;        
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        _rigidbody.velocity = direction;
        animationHandler.Move(direction);
    }

    private void Rotate(Vector2 direction)
    {
        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotationZ) > 90.0f;

        characterRenderer.flipX = isLeft;

        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotationZ);
        }

        weaponHandler?.Rotate(isLeft);
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
    }

    private void HandleAttackDelay()
    {
        if (weaponHandler == null) return;

        if (timeSinceLastAttack <= weaponHandler.AttackDelay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }
        
        if(isAttacking && timeSinceLastAttack > weaponHandler.AttackDelay)
        {
            timeSinceLastAttack = 0.0f;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if (lookDirection != Vector2.zero)
        {
            weaponHandler?.Attack();
        }
    }

    public virtual void Died()
    {
        if (this is EnemyController)
        {
            if (PlayerStats.Instance != null && statHandler != null)
            {
                PlayerStats.Instance.AddGold(statHandler.RewardGold);
                PlayerStats.Instance.AddExp(statHandler.RewardExp);
            }
        }

        _rigidbody.velocity = Vector3.zero;

        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.material = dieMaterial;
        }

        //Color testColor = testSprite.color;
        //testColor.a = 0.3f;  // 투명
        //testSprite.color = testColor;

        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }
    }

    // 내가 몰랐떤 이유: 내가 한 게 아님(내 스크립트가 아님)
    // 유니티 내부 Sprite가 애니메이터가 < 색을 못바꿔 ?.?

    // 리셋 함수
    public virtual void Reset()
    {
        // 초기 상태로 복원
        movementDirection = initialMovementDirection;
        lookDirection = initialLookDirection;
        isAttacking = initialIsAttacking;
        //statHandler.Health = (int)initialHealth;
        resourceController.CurrentHealth = initialHealth;
        knockbackDuration = initialKnockbackDuration;

        // Rigidbody 초기화: 초기 이동 방향으로 설정
        _rigidbody.velocity = movementDirection * statHandler.Speed;

        // 모든 Behaviour 컴포넌트를 활성화
        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = true;
        }


        // SpriteRenderer 복원 (불투명으로 설정)
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.material = resetMaterial;
        }

    }

    // 사망한 물체를 어떻게 처리할지에 대한 함수
    protected virtual void OnDeathComplete()
    {
        Destroy(gameObject, 2f);
    }
}
