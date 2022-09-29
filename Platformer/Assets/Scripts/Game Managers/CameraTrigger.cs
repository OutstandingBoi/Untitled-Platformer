using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    CinemachineVirtualCamera cam;

    void Awake()
    {
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (CameraManager.activeCamera != cam)
                CameraManager.SwitchCamera(cam);
        }
    }
}
