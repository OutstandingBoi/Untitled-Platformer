using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject attackObject;
    Rigidbody2D body;
    RaycastGroundDetection ground;
    PlayerToggleCollision ghost;
    TrailRenderer trail;
    SpriteRenderer sr;
    public Vector2 attackDirection;

    [Header("AttackStats")]
    [SerializeField, Range(0f, 50f)][Tooltip("Dash distance")] float dashDistance = 14f;
    [SerializeField, Range(0f, 1f)][Tooltip("Time until attack is complete")] float attackTime = 0.5f;
    [SerializeField, Range(0f, 5f)][Tooltip("Time until dash is available again")] float dashCD = 0.5f;
    [SerializeField, Range(0, 1f)][Tooltip("Grace period for inputing an attack")] float attackBuffer = 0.2f;

    [Header("Current State")]
    public bool isAttacking;
    public bool isDashing;
    [SerializeField] int attackPhase = 0;
    bool desiredAttack;
    bool canAttack = true;
    bool canDash = true;
    bool ghosting;
    bool onGround;
    float attackBufferTimer;

    void Awake()
    {
        //Find the player's components and assigns them to variables
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<RaycastGroundDetection>();
        ghost = GetComponent<PlayerToggleCollision>();
        trail = GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        ghosting = ghost.GetGhosting();

        if (onGround && !isDashing)
            attackPhase = 0;

        AttackBuffer();
    }

    void FixedUpdate()
    {
        onGround = ground.GetOnGround();

        if (desiredAttack)
        {
            switch (ghosting)
            {
                case true: 
                    StartCoroutine(DashCoroutine(dashCD));
                    break;
                case false:
                    StartCoroutine(AttackCoroutine(attackTime));
                    break;
            }
        }
    }

    IEnumerator AttackCoroutine(float cooldown)
    {
        if (canAttack)
        {
            attackObject.SetActive(true);
            canAttack = false;
            desiredAttack = false;
            isAttacking = true;

            if (attackPhase == 0)
            {
                StartCoroutine(DashCoroutine(attackTime));
            }
            else if (!onGround)
                body.velocity = new Vector2(attackDirection.normalized.x * dashDistance, attackDirection.normalized.y 
                    * Mathf.Sqrt(dashDistance));

            attackPhase += 1;

            yield return new WaitForSeconds(cooldown);
            attackObject.SetActive(false);
            canAttack = true;
            isAttacking = false;
        }
    }

    IEnumerator DashCoroutine(float cooldown)
    {
        if (canDash || isAttacking)
        {
            canDash = false;
            //Causes the player to dash forward if they are on the ground, or if its their first attack in the air
            isDashing = true;
            trail.emitting = true;

            //Changes the player's veloctiy based on their input and dash distance
            body.velocity = attackDirection.normalized * dashDistance;

            yield return new WaitForSeconds(attackTime);
            isDashing = false;
            trail.emitting = false;

            yield return new WaitForSeconds(cooldown);
            canDash = true;
        }
    }

    private void AttackBuffer()
    {
        //Adds a slight buffer before the attack input is ignored
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

    //Input Methods
    #region
    public void OnMouseDirection(InputAction.CallbackContext context)
    {
        //Only fires code if the camera is active to prevent errors
        if (Camera.main != null)
        {
            //Checks player and mouse position and stores them as variables
            Vector2 playerPosition = Camera.main.WorldToViewportPoint(transform.position);
            Vector2 mousePosition = Camera.main.ScreenToViewportPoint(context.ReadValue<Vector2>());

            float angle = AngleBetweenTwoPoints(playerPosition, mousePosition);

            //Changes the rotation of the attack object so that the player slashes toward their mouse, but only when not attacking
            if (!isAttacking)
                attackObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            //Stores a direction based on the mouse's and player's positions
            attackDirection = mousePosition - playerPosition;
        }
    }

    public void OnGamepadDirection(InputAction.CallbackContext context)
    {
        if (Camera.main != null)
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
    #endregion
}
