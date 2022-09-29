using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class CameraManager
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    public static CinemachineVirtualCamera activeCamera = null;

    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        camera.Priority = 10;
        activeCamera = camera;

        foreach (CinemachineVirtualCamera c in cameras)
        {
            if (c != camera && c.Priority != 0)
                c.Priority = 0;
        }
    }

    public static void RegisterCameras()
    {
        if (Object.FindObjectsOfType<CinemachineVirtualCamera>() != null)
        {
            foreach (CinemachineVirtualCamera camera in Object.FindObjectsOfType<CinemachineVirtualCamera>())
                cameras.Add(camera);
        }
    }

    public static void UnregisterCameras()
    {
        if (cameras.Count > 0)
        {
            cameras = new List<CinemachineVirtualCamera>();
        }
    }
}
