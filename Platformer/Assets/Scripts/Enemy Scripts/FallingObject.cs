using UnityEngine;

public class FallingObject : MonoBehaviour
{
    bool startFalling;
    bool grounded;

    RaycastGroundDetection ground;
    Rigidbody2D body;
    Attack attack;

    void Awake()
    {
        ground = GetComponent<RaycastGroundDetection>();
        body = GetComponent<Rigidbody2D>();
        attack = GetComponent<Attack>();
        body.bodyType = RigidbodyType2D.Static;
    }

    void FixedUpdate()
    {
        grounded = ground.GetOnGround();

        if (startFalling && !grounded)
        {
            attack.damage = 100;
            body.bodyType = RigidbodyType2D.Dynamic;  
        }
        else
        {
            attack.damage = 0;
            body.bodyType = RigidbodyType2D.Static;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            startFalling = true;
    }
}
