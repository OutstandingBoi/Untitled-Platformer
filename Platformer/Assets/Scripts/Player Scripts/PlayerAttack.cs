using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject attackObject;
    Rigidbody2D body;
    PlayerGround ground;
    TrailRenderer trail;
    SpriteRenderer sr;
    public Vector2 attackDirection;

    [Header("AttackStats")]
    [SerializeField, Range(0f, 50f)] [Tooltip("Dash distance")] float dashDistance = 14f;
    [SerializeField, Range(0f, 1f)] [Tooltip("Time until dash is complete")] float attackTime = 0.5f;
    [SerializeField, Range(0, 1f)] [Tooltip("Grace period for inputing an attack")] float attackBuffer = 0.2f;

    [Header("Current State")]
    public bool isAttacking;
    public bool isDashing;
    [SerializeField] int attackPhase = 0;
    bool desiredAttack;
    bool canAttack = true;
    bool onGround;
    float attackBufferTimer;

    void Awake()
    {
        //Find the player's Rigidbody, Ground, and Trail Renderer
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<PlayerGround>();
        trail = GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        onGround = ground.GetOnGround();

        if (onGround && !isDashing)
            attackPhase = 0;

        //Adds a slight buffer before the jump input is ignored
        if (desiredAttack)
        {
            attackBufferTimer += Time.deltaTime;

            if (attackBufferTimer > attackBuffer)
            {
                desiredAttack = false;
                attackBufferTimer = 0;
            }
        }
    }

    void FixedUpdate()
    {
        if (desiredAttack)
            StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        if (canAttack)
        {
            attackObject.SetActive(true);
            canAttack = false;
            desiredAttack = false;
            isAttacking = true;

            //Causes the player to dash forward if they are on the ground, or if its their first attack in the air
            if (attackPhase == 0)
            {
                isDashing = true;
                trail.emitting = true;
                //Changes the player's veloctiy based on their input and dash distance
                body.velocity = attackDirection.normalized * dashDistance;
            }
            else if (!onGround)
                body.velocity = new Vector2(attackDirection.normalized.x * dashDistance, 
                    attackDirection.normalized.y * Mathf.Sqrt(dashDistance));

            attackPhase += 1;

            yield return new WaitForSeconds(attackTime);
            attackObject.SetActive(false);
            canAttack = true;
            isAttacking = false;
            isDashing = false;
            trail.emitting = false;
        }
    }

    public void OnMouseDirection(InputAction.CallbackContext context)
    {
        Vector2 playerPosition = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 mousePosition = Camera.main.ScreenToViewportPoint(context.ReadValue<Vector2>());

        float angle = AngleBetweenTwoPoints(playerPosition, mousePosition);

        //Changes the rotation of the attack object so that the player slashes toward their mouse, but only when not attacking
        if (!isAttacking)
            attackObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //Stores a direction based on the mouse's and player's positions
        attackDirection = mousePosition - playerPosition;
    }

    public void OnGamepadDirection(InputAction.CallbackContext context)
    {
        //Reads the directional input of the player as well as their current position
        Vector2 playerPosition = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 gamepadDir = context.ReadValue<Vector2>().normalized;

        //If player is not inputting a direction, then the direction will default to forward
        if (gamepadDir.x == 0 && gamepadDir.y == 0)
            gamepadDir.x = sr.flipX ? -1 : 1;

        float angle = AngleBetweenTwoPoints(playerPosition, playerPosition + gamepadDir.normalized);

        //Changes the rotation of the attack object so that the player slashes toward their mouse, but only when not attacking
        if (!isAttacking)
            attackObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //Stores the x and y values into a normalized vector
        attackDirection = gamepadDir;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //Tracks player input of the attack button
        if (context.started)
        {
            sr.flipX = attackDirection.x < 0;
            desiredAttack = true;
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
