using UnityEngine;

public class FallingObject : MonoBehaviour
{
    bool startFalling;
    bool grounded;

    Interactable triggerBox;
    RaycastGroundDetection ground;
    Rigidbody2D body;
    Attack attack;

    void Awake()
    {
        GetComponentInChildren<Interactable>().interacted += OnTriggerBox;
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

    void OnTriggerBox()
    {
        startFalling = true;
    }
}
