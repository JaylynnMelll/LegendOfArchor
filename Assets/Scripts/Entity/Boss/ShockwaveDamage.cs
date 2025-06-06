using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveDamage : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float damage = 3f;
    [SerializeField] private float delayBeforeDamage = 0.5f; // 파티클 끝나는 시간과 맞추기
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private float slowDownRate = 2f;
    [SerializeField] private LayerMask targetLayer;

    [Header("카메라 흔들림")]
    [SerializeField] private float shakeIntensity = 0.4f;
    [SerializeField] private float shakeDuration = 0.25f;

    [Header("오디오")]
    [SerializeField] private AudioClip chargingSoundClip;
    [SerializeField] private AudioClip explosionSoundClip;

    private Transform playerTarget;
    private float currentSpeed;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        currentSpeed = initialSpeed;

        if(chargingSoundClip != null)
        {
            audioSource.clip = chargingSoundClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        // 플레이어 서칭
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        Debug.Log("Start called");

        StartCoroutine(DelayedDamage());
    }

    private void Update()
    {
        // 플레이어 유도
        if (playerTarget != null)
        {
            Vector2 dir = (playerTarget.position - transform.position).normalized;
            transform.position += (Vector3)dir * currentSpeed * Time.deltaTime;

            currentSpeed = Mathf.Max(0f, currentSpeed - slowDownRate * Time.deltaTime);

            Debug.Log("충격파 이동");
        }
        else 
        {
            Debug.Log("playerTarget이 null입니다");
        }
    }

    private IEnumerator DelayedDamage()
    {
        yield return new WaitForSeconds(delayBeforeDamage);

        // 범위 데미지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        foreach (var hit in hits)
        {
            ResourceController rc = hit.GetComponent<ResourceController>();
            if (rc != null)
            {
                rc.ChangeHealth(-damage);
                Debug.Log("충격파 발생 " + rc.name);
            }
        }

        if (explosionSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionSoundClip, transform.position);
        }

        // 카메라 흔들림
        PlayerCamera.Instance?.Shake(shakeIntensity, shakeDuration);

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
