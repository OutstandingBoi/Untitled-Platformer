using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] List<GameObject> levelEnemies = new List<GameObject>();
    [SerializeField] Canvas gameOverUI;
    [SerializeField] Text loseText, victoryText;
    string currentScene;
    GameObject player;
    bool playerAlive;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        gameOverUI.enabled = false;
        loseText.enabled = false;
        victoryText.enabled = false;
    }

    void Update()
    {
        if (player != null)
        {
            playerAlive = player.GetComponent<Health>().alive;

            if (playerAlive == false)
            {
                gameOverUI.enabled = true;
                loseText.enabled = true;
            }
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Uses the camera manager static class to add all cameras in the scene to a list
        CameraManager.RegisterCameras();

        currentScene = scene.name;

        RegisterEnemies();

        //Gets a reference to the player object
        if (GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnSceneUnloaded(Scene current)
    {
        //Resets the camera manager's list of cameras
        CameraManager.UnregisterCameras();

        //Resets the level enemies list as well as the player object reference
        levelEnemies = new List<GameObject>();
        player = null;
    }

    void RegisterEnemies()
    {
        //Adds all objects tagged Enemy to a list of current enemies
        if (GameObject.FindGameObjectsWithTag("Enemy") != null)
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                levelEnemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        levelEnemies.Remove(enemy);
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
