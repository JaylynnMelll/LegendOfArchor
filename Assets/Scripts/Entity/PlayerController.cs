using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 1.0f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask enemyProjectileLayer;
    [SerializeField] private LayerMask obstacleMask;
    private ContactFilter2D contactFilter;
    private Collider2D playerCollider;

    private bool isDodging = false;
    public bool IsDodging => isDodging;
    private float lastDodgeTime = -Mathf.Infinity;
    private Vector2 dodgeDirection = Vector2.zero;

    private Camera camera;
    private GameManager gameManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        camera = Camera.main;
        playerCollider = GetComponent<Collider2D>();
    }

    protected override void HandleAction()
    {
        if (isDodging) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementDirection = new Vector2(horizontal, vertical).normalized;

        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPosition = camera.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPosition - (Vector2)transform.position);

        if (lookDirection.magnitude < 0.9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }

        isAttacking = Input.GetMouseButton(0);

        if (Input.GetKeyDown(KeyCode.Space) && !isDodging && Time.time >= lastDodgeTime + dodgeCooldown)
        {
            StartCoroutine(Dodge());
        }
    }

    private IEnumerator Dodge()
    {
        isDodging = true;
        lastDodgeTime = Time.time;

        dodgeDirection = movementDirection != Vector2.zero ? movementDirection : lookDirection;
        if (dodgeDirection == Vector2.zero) dodgeDirection = Vector2.right;

        // 구르기 애니메이션 추가 예정

        SetCollisionWithEnemies(false); // 무적

        // 도약 지점 계산 로직
        Vector2 start = transform.position;
        Vector2 direction = dodgeDirection.normalized;
        Vector2 targetPos = start + direction * dodgeDistance;

        // 장애물 감지(벽)
        RaycastHit2D hit = Physics2D.Raycast(start, direction, dodgeDistance, obstacleMask);
        if (hit.collider != null)
        {
            targetPos = hit.point; // 충돌 위치까지 이동
        }

        float elapsed = 0f;
        Vector2 initialPos = transform.position;

        while (elapsed < dodgeDuration)
        {
            transform.position = Vector2.Lerp(initialPos, targetPos, elapsed / dodgeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        isDodging = false;
        SetCollisionWithEnemies(true);
    }
    public override void Died()
    {
        base.Died();
        gameManager.GameOver();
    }

    void OnMove(InputValue inputValue)
    {
        movementDirection = inputValue.Get<Vector2>();
        movementDirection = movementDirection.normalized;
    }

    void OnLook(InputValue inputValue)
    {
        Vector2 mousePosition = inputValue.Get<Vector2>();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);

        if (lookDirection.magnitude < .9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }
    }

    void OnFire(InputValue inputValue)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        isAttacking = inputValue.isPressed;
    }

    void OnPause()
    {
        if (gameManager != null && gameManager.IsGamePlaying())
        {
            gameManager.PauseGame();
        }
    }

    private void SetCollisionWithEnemies(bool enable)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), !enable);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyProjectile"), !enable);
    }
}


