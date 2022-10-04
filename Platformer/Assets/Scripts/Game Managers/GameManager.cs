using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    string currentScene;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Uses the camera manager static class to add all cameras in the scene to a list
        CameraManager.RegisterCameras();

        currentScene = scene.name;
    }

    void OnSceneUnloaded(Scene current)
    {
        //Resets the camera manager's list of cameras
        CameraManager.UnregisterCameras();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentScene);
    }
}
