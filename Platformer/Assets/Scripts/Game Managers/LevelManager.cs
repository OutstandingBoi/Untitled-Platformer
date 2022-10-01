using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelManager : MonoBehaviour
{
    enum ObjectiveType { None = 4, LockedDoor = 1, Boss = 2, KillAllEnemies = 3, Tutorial = 5 }

    [Header("UI Objects")]
    [SerializeField] Canvas gameOverUI;
    [SerializeField] Text victoryText;

    [Header("Level Objective Type")]
    [SerializeField] ObjectiveType objective;
    //Locked door objective variables
    [SerializeField, HideInInspector] GameObject door, key, exit;
    [SerializeField, HideInInspector] bool hasKey;
    //Boss fight objective variables
    [SerializeField, HideInInspector] GameObject boss;
    //Kill all enemies objective variables
    [SerializeField, HideInInspector] List<GameObject> levelEnemies = new List<GameObject>();

    void Awake()
    {
        //Checks which level objective is set
        switch (objective)
        {
            case ObjectiveType.None:
                return;
            //Subscribes to interaction events for the key, door, and exit objects
            case ObjectiveType.LockedDoor:
                key.GetComponent<Interactable>().interacted += OnKeyInteracted;
                door.GetComponent<Interactable>().interacted += OnDoorInteracted;
                exit.GetComponent<Interactable>().interacted += OnExitInteracted;
                break;

            case ObjectiveType.Boss:
                break;

            case ObjectiveType.KillAllEnemies:
                break;
            case ObjectiveType.Tutorial:
                key.GetComponent<Interactable>().interacted += OnKeyInteracted;
                door.GetComponent<Interactable>().interacted += OnDoorInteracted;
                exit.GetComponent<Interactable>().interacted += OnExitInteracted;
                break;
        }
    }

    //Is called when the player interacts with the gameobject set to exit
    private void OnExitInteracted()
    {
        //enables the level complete UI elements
        gameOverUI.enabled = true;
        victoryText.enabled = true;
    }

    //Is called when the player interacts with the gameobject set to key
    private void OnKeyInteracted()
    {
        //Sets the hasKey boolean to true if the player does not already have it and then disables the key object
        if (!hasKey)
            hasKey = true;
        key.SetActive(false);
    }

    //Is called when the player interacts with the gameobject set to door
    private void OnDoorInteracted()
    {
        //Is called when the 
        if (hasKey)
        {
            hasKey = false;
            door.SetActive(false);
        }
    }

    #region editor
#if UNITY_EDITOR

    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LevelManager levelManager = (LevelManager)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Objective Objects", EditorStyles.boldLabel);

            //Checks what the level's objective is set to and draws its variables in the inspector
            switch (levelManager.objective)
            {
                case ObjectiveType.LockedDoor:
                    DoorVariables(levelManager);
                    break;

                case ObjectiveType.Boss:
                    //Variables
                    break;

                case ObjectiveType.KillAllEnemies:
                    //Variables
                    break;
                case ObjectiveType.Tutorial:
                    DoorVariables(levelManager);
                    break;
            }
        }

        private static void DoorVariables(LevelManager levelManager)
        {
            //Sets up serialized variables in the Unity inspector
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Door", GUILayout.MaxWidth(70));
            levelManager.door = (GameObject)EditorGUILayout.ObjectField(levelManager.door, typeof(GameObject), true);
            EditorGUILayout.LabelField("Key", GUILayout.MaxWidth(70));
            levelManager.key = (GameObject)EditorGUILayout.ObjectField(levelManager.key, typeof(GameObject), true);
            EditorGUILayout.LabelField("Exit", GUILayout.MaxWidth(70));
            levelManager.exit = (GameObject)EditorGUILayout.ObjectField(levelManager.exit, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Has Key", GUILayout.MaxWidth(70));
            levelManager.hasKey = EditorGUILayout.Toggle(levelManager.hasKey, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }
    }

#endif
    #endregion
}
