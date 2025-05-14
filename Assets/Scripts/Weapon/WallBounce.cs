using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    [SerializeField] [Range (0f, 1f)] private float bounceForce = 0.75f; // BounceForce (0 ~ 1)
    [SerializeField] private LayerMask levelCollisionLayer;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 상대 오브젝트가 Wall 레이어인 경우
        if (collision.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            // 충돌한 상대 오브젝트의 Normal 벡터를 구함.
            Vector2 collisionNormal = (transform.position - collision.transform.position).normalized;

            // 충돌한 상대 오브젝트의 Normal 벡터를 기준으로 반사된 벡터를 구함.
            Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, collisionNormal);

            // 반사된 벡터에 BounceForce를 곱하여 속도를 조정함.
            rb.velocity = reflectedVelocity * bounceForce;
        }
    }

}
