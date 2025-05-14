using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpinningMeleeProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 1f;
    public float rotationSpeed = 720f;
    public float hitCooldown = 0.2f;
    private float radius = 1.5f;

    private float damage = 5f;
    private float criticalChance = 0f;
    private float criticalDamage = 1f;

    private LayerMask targetMask;
    private Transform center;
    private Transform visual;
    private float timeElapsed = 0f;

    [Header("SkillData Reference")]
    [SerializeField] private Skill weaponSizeEnlargedSkill;
    [SerializeField] private Skill attackSpeedBoostSkill;
    [SerializeField] private Skill parryingSkill;

    private Dictionary<Collider2D, float> hitCooldowns = new Dictionary<Collider2D, float>();

    public void Init(
        Transform owner,
        LayerMask target,
        LayerMask wallMask, // reserved for future bounce
        float finalDamage,
        float finalRange,
        float critChance,
        float critDamageMult,
        Sprite weaponSprite,
        float angleOffset = 0f
    )
    {
        this.center = owner;
        this.targetMask = target;
        this.damage = finalDamage;
        this.radius = finalRange;
        this.criticalChance = critChance;
        this.criticalDamage = critDamageMult;

        visual = transform.Find("Visual");
        if (visual != null)
        {
            Vector3 offset = Quaternion.Euler(0, 0, angleOffset - 90f) * Vector3.right * radius;
            visual.localPosition = offset;

            SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
            if (sr != null && weaponSprite != null)
            {
                sr.sprite = weaponSprite;
                sr.color = new Color(1f, 1f, 1f, 0.8f);
            }
        }

        transform.SetParent(null);
        Destroy(gameObject, duration);
        ApplyEnlargingWeaponSize();
        ApplyAttackSpeedBoost();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration)
        {
            Destroy(gameObject);
            return;
        }

        if (center != null)
        {
            transform.RotateAround(center.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        if (visual != null)
        {
            visual.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        float hitRadius = radius * 0.8f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, targetMask);
        foreach (var hit in hits)
        {
            ApplydParryingSkill(hit);
        }

        CheckParryAura();
    }

    private void ApplyEnlargingWeaponSize()
    {
        PlayerSkillHandler handler = FindObjectOfType<PlayerSkillHandler>();
        if (handler == null || weaponSizeEnlargedSkill == null) return;

        RuntimeSkill stackInfo = handler.trackingList.FirstOrDefault(s => s.skill == weaponSizeEnlargedSkill);
        if (stackInfo == null) return;

        int stack = Mathf.Clamp(stackInfo.currentStacks, 0, 5);
        float multiplier = 1f + 0.2f * stack;

        // Scale radius
        radius *= multiplier;

        // Scale the visual
        if (visual != null)
        {
            visual.localScale *= multiplier;
        }

#if UNITY_EDITOR
        Debug.Log($"[SpinSlash] Radius: {radius}");
        Debug.Log($"[SpinSlash] Visual Scale: {visual.localScale}");
#endif
    }

    private void ApplyAttackSpeedBoost()
    {
        PlayerSkillHandler handler = FindObjectOfType<PlayerSkillHandler>();
        if (handler == null || attackSpeedBoostSkill == null) return;

        RuntimeSkill stackInfo = handler.trackingList.FirstOrDefault(s => s.skill == attackSpeedBoostSkill);
        if (stackInfo == null) return;

        int stack = Mathf.Clamp(stackInfo.currentStacks, 0, 5);
        float multiplier = 1f + 0.3f * stack; // 50% per stack

        rotationSpeed *= multiplier;

#if UNITY_EDITOR
        Debug.Log($"[SpinSlash] Attack Speed Multiplier: {multiplier}");
        Debug.Log($"[SpinSlash] New Rotation Speed: {rotationSpeed}");
#endif
    }

    private void ApplydParryingSkill(Collider2D hit)
    {
        if (hitCooldowns.TryGetValue(hit, out float nextTime) && Time.time < nextTime)
            return;

        // Enemy damage
        var resourceController = hit.GetComponent<ResourceController>();
        if (resourceController != null)
        {
            float finalDmg = damage;
            if (Random.value < criticalChance)
                finalDmg *= criticalDamage;

            resourceController.ChangeHealth(-finalDmg);
            hitCooldowns[hit] = Time.time + hitCooldown;

#if UNITY_EDITOR
            Debug.Log($"[SpinSlash] {hit.name}에게 {finalDmg} 데미지!");
#endif
        }
        // Parry projectile
        else if (HasParryingSkill() && hit.CompareTag("Projectile"))
        {
            Destroy(hit.gameObject);

#if UNITY_EDITOR
            Debug.Log($"[Parry] Destroyed projectile: {hit.name}");
#endif
        }

    }
    
    private bool HasParryingSkill()
    {
        PlayerSkillHandler handler = FindObjectOfType<PlayerSkillHandler>();
        if (handler == null || parryingSkill == null) return false;

        return handler.acquiredSkills.Contains(parryingSkill);
    }

    private void CheckParryAura()
    {
        if (!HasParryingSkill()) return;

        Collider2D[] parryTargets = Physics2D.OverlapCircleAll(transform.position, radius * 0.8f);
        foreach (var obj in parryTargets)
        {
            if (obj.CompareTag("Projectiles"))
            {
                Destroy(obj.gameObject);

#if UNITY_EDITOR
                Debug.Log($"[Parry Aura] Destroyed projectile: {obj.name}");
#endif
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius * 0.8f);
    }
}

  