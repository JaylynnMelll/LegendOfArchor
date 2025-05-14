using System.Collections.Generic;
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
            if (hitCooldowns.TryGetValue(hit, out float nextTime) && Time.time < nextTime)
                continue;

            var rc = hit.GetComponent<ResourceController>();
            if (rc != null)
            {
                float finalDmg = damage;
                if (Random.value < criticalChance)
                    finalDmg *= criticalDamage;

                rc.ChangeHealth(-finalDmg);
                hitCooldowns[hit] = Time.time + hitCooldown;

#if UNITY_EDITOR
                Debug.Log($"[SpinSlash] {hit.name}에게 {finalDmg} 데미지!");
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