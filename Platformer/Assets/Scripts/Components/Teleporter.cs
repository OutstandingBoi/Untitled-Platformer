using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Transform destination;

    private void Awake()
    {
        var cHeight = 2 * Camera.main.orthographicSize;
        var cWidth = (cHeight) * Camera.main.aspect;

        Debug.Log($"The camera is {cHeight} units tall and {cWidth} units wide.");
    }

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
