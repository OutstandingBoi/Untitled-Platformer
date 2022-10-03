using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection.Emit;

#if UNITY_EDITOR
using UnityEditor;
#endif


//This script is used by both movement and jump to detect when the character is touching the ground

public class RaycastGroundDetection : MonoBehaviour
{
    PlayerToggleCollision ghost;

    [SerializeField] bool isPlayer;
    [SerializeField] bool checkGround, checkWalls;
    bool goingGhost;

    [SerializeField, HideInInspector] bool onGround;
    [SerializeField, HideInInspector] bool onWallLeft;
    [SerializeField, HideInInspector] bool onWallRight;

    [Header("Collider Settings")]
    [SerializeField, HideInInspector][Tooltip("Length of the ground-checking collider")] float groundLength;
    [SerializeField, HideInInspector][Tooltip("Length of the wall-checking collider")] float wallLength;
    [SerializeField, HideInInspector][Tooltip("Distance between the ground-checking colliders")] Vector3 groundColliderOffset;
    [SerializeField, HideInInspector][Tooltip("Distance between the wall-checking colliders")] Vector3 wallColliderOffset;

    [Header("Layer Masks")]
    [SerializeField][Tooltip("Which layers are read as the ground")] LayerMask groundLayer;
    [SerializeField][Tooltip("Which layers are read as impassable ground")] LayerMask solidGroundLayer;

    private void Awake()
    {
        if (isPlayer)
            ghost = GetComponent<PlayerToggleCollision>();
    }

    void Update()
    {
        if (isPlayer)
            goingGhost = ghost.GetGhosting();

        //Determine if the object is stood on objects on the ground layer, using a series of raycasts
        onGround = DualRaycast(groundColliderOffset, Vector2.down, groundLength);

        //Determine if the object is touching a wall
        onWallLeft = DualRaycast(wallColliderOffset, Vector2.left, wallLength);
        onWallRight = DualRaycast(wallColliderOffset, Vector2.right, wallLength);
    }

    void OnDrawGizmos()
    {
        //Draw the ground colliders on screen for debug purposes
        if (onGround || onWallLeft || onWallRight) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        DrawGizmoLine(groundColliderOffset, Vector3.down, groundLength);
        DrawGizmoLine(wallColliderOffset, Vector3.right, wallLength);
        DrawGizmoLine(wallColliderOffset, Vector3.left, wallLength);
    }

    bool DualRaycast(Vector3 offset, Vector2 direction, float length)
    {
        LayerMask groundLayerMask;

        if (isPlayer) groundLayerMask = goingGhost ? solidGroundLayer : groundLayer;
        else groundLayerMask = groundLayer;

        return Physics2D.Raycast(boxPosition(true, offset), direction, length, groundLayerMask) ||
               Physics2D.Raycast(boxPosition(false, offset), direction, length, groundLayerMask);
    }

    void DrawGizmoLine(Vector3 offset, Vector3 direction, float length)
    {
        Gizmos.DrawLine(boxPosition(true, offset), boxPosition(true, offset) + direction * length);
        Gizmos.DrawLine(boxPosition(false, offset), boxPosition(false, offset) + direction * length);
    }

    //Send ground detection to other scripts
    public bool GetOnGround() { return onGround; }
    public bool GetOnWallLeft() { return onWallLeft; }
    public bool GetOnWallRight() { return onWallRight; }

    private Vector3 boxPosition(bool isLeftRay, Vector3 offset) 
    { 
        return new Vector3(isLeftRay ? transform.position.x + offset.x : transform.position.x - offset.x,
            isLeftRay ? transform.position.y + offset.y : transform.position.y - offset.y, transform.position.z + offset.z); 
    }


    #region editor
#if UNITY_EDITOR
    [CustomEditor(typeof(RaycastGroundDetection))]
    public class GroundDetectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            RaycastGroundDetection groundDetection = (RaycastGroundDetection)target;
            Undo.RecordObject(target, "");

            EditorGUILayout.Space();

            if (groundDetection.checkGround)
            {
                //DetectionVariables("Ground Detection", groundDetection.groundLength, groundDetection.groundColliderOffset);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Ground Detection", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Length", GUILayout.MaxWidth(200));
                groundDetection.groundLength = EditorGUILayout.Slider(groundDetection.groundLength, 0f, 1f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                groundDetection.groundColliderOffset = EditorGUILayout.Vector3Field("Offset", groundDetection.groundColliderOffset);
                EditorGUILayout.EndHorizontal();
            }
            if (groundDetection.checkWalls)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Wall Detection", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Length", GUILayout.MaxWidth(200));
                groundDetection.wallLength = EditorGUILayout.Slider(groundDetection.wallLength, 0f, 1f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                groundDetection.wallColliderOffset = EditorGUILayout.Vector3Field("Offset", groundDetection.wallColliderOffset);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
    #endregion
}
