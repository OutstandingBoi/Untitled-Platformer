using UnityEngine;

public class PlayerWallDetection : MonoBehaviour
{
    [SerializeField] bool inWall;
    [SerializeField][Tooltip("Which layers are read as the ground")] LayerMask groundLayer;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            inWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            inWall = false;
        }
    }

    public bool GetInWall() { return inWall; }
}
