using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Interacted();

public class Interactable : MonoBehaviour
{
    new Collider2D collider;
    public event Interacted interacted;

    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collider != null && collider.isTrigger)
        {
            if (collision.CompareTag("Player"))
            {
                OnInteracted();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collider != null && !collider.isTrigger)
        {
            if (collision.collider.CompareTag("Player"))
                OnInteracted();
        }
    }

    protected virtual void OnInteracted()
    {
        interacted?.Invoke();
    }
}
