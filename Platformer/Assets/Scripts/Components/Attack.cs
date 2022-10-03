using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    Collider2D attackCollider;

    [Header("Attack Stats")]
    [SerializeField, Range(0f, 100f)] [Tooltip("The amount of damage the attack deals")] public float damage;

    void Awake()
    {
        attackCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(attackCollider != null && attackCollider.isTrigger == true)
        {
            if (collision.GetComponent<Health>() != null)
            {
                Health target = collision.GetComponent<Health>();
                target.TakeDamage(damage);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (attackCollider != null && attackCollider.isTrigger == false)
        {
            if (collision.collider.GetComponent<Health>() != null)
            {
                Health target = collision.collider.GetComponent<Health>();
                target.TakeDamage(damage);
            }

        }

    }
}
