using UnityEngine;

public class PlayerWallDetection : MonoBehaviour
{
    public bool inWall;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            inWall = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            inWall = false;
        }
    }
}
