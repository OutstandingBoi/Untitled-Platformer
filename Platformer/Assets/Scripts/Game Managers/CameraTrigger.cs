using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    [SerializeField] int cameraNumber;
    [SerializeField] List<Text> text = new List<Text>();
    [SerializeField] bool isTutorial = false;

    void Awake()
    {
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (CameraManager.activeCamera != cam)
            {
                CameraManager.SwitchCamera(cam);

                //Switches text if labeled to be a tutorial
                if (isTutorial)
                {
                text[cameraNumber].enabled = true;
                foreach (Text t in text)
                {
                    if (t != text[cameraNumber])
                        t.enabled = false;
                }

                }
            }
        }
    }
}
