using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Transform destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destination != null)
        {
            if (collision.CompareTag("Player"))
            {
                collision.transform.position = destination.position;
            }
        }
    }
}
