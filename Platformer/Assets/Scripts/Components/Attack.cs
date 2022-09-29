using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Attack Stats")]
    [SerializeField, Range(0f, 100f)] [Tooltip("The amount of damage the attack deals")] float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Health>() != null)
        {
            Health target = collision.GetComponent<Health>();
            target.TakeDamage(damage);
        }
    }
}
