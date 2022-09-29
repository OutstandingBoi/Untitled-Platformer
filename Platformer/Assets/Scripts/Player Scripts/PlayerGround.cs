using UnityEngine;

//This script is used by both movement and jump to detect when the character is touching the ground

public class PlayerGround : MonoBehaviour
{
    PlayerToggleCollision ghost;

    [SerializeField] bool onGround;
    [SerializeField] bool onWallLeft;
    [SerializeField] bool onWallRight;
    bool goingGhost;

    [Header("Collider Settings")]
    [SerializeField] [Tooltip("Length of the ground-checking collider")] float groundLength = 0.95f;
    [SerializeField] [Tooltip("Length of the wall-checking collider")] float wallLength = 0.95f;
    [SerializeField] [Tooltip("Distance between the ground-checking colliders")] Vector3 groundColliderOffset;
    [SerializeField] [Tooltip("Distance between the wall-checking colliders")] Vector3 wallColliderOffset;

    [Header("Layer Masks")]
    [SerializeField] [Tooltip("Which layers are read as the ground")] LayerMask groundLayer;
    [SerializeField] [Tooltip("Which layers are read as impassable ground")] LayerMask solidGroundLayer;

    private void Awake()
    {
        ghost = GetComponent<PlayerToggleCollision>();
    }

    void Update()
    {
        goingGhost = ghost.GetGhosting();

        //Determine if the player is stood on objects on the ground layer, using a series of raycasts
        onGround = DualRaycast(groundColliderOffset, Vector2.down, groundLength);

        //Determine if the player is touching a wall
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
        return Physics2D.Raycast(transform.position + offset, direction, length, goingGhost ? solidGroundLayer : groundLayer) ||
               Physics2D.Raycast(transform.position - offset, direction, length, goingGhost ? solidGroundLayer : groundLayer);
    }

    void DrawGizmoLine(Vector3 offset, Vector3 direction, float length)
    {
        Gizmos.DrawLine(transform.position + offset, transform.position + offset + direction * length);
        Gizmos.DrawLine(transform.position - offset, transform.position - offset + direction * length);
    }

    //Send ground detection to other scripts
    public bool GetOnGround() { return onGround; }
    public bool GetOnWallLeft() { return onWallLeft; }
    public bool GetOnWallRight() { return onWallRight; }
}