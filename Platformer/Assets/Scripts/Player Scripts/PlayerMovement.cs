using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D body;
    PlayerGround ground;
    PlayerAttack dash;
    PlayerToggleCollision ghost;
    SpriteRenderer sr;

    [Header("Movement Stats")]
    [SerializeField, Range(0f, 100f)][Tooltip("Maximum movement speed")] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed")] float maxAcceleration = 70f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed when in mid-air")] float maxAirAcceleration = 70f;

    [Header("Jumping Stats")]
    [SerializeField, Range(0f, 10f)][Tooltip("Maximum jump height")] float jumpHeight = 6f;
    [SerializeField, Range(0, 1f)][Tooltip("How long coyote time lasts")] float coyoteTime = 0.2f;
    [SerializeField, Range(0, 1f)][Tooltip("Grace period for inputing a jump while falling")] float jumpBuffer = 0.2f;
    [SerializeField, Range(0f, 30f)][Tooltip("Distance when jumping off a wall")] float wallJumpSpeed = 17f;

    [Header("Physics Stats")]
    [SerializeField, Range(0f, 10f)][Tooltip("Gravity multiplier to apply when going up")] float upwardMovementMultiplier = 4f;
    [SerializeField, Range(0f, 10f)][Tooltip("Gravity multiplier to apply when coming down")] float downwardMovementMultiplier = 5f;
    [SerializeField, Range(0f, 10f)][Tooltip("Gravity multiplier to apply when using ghost powers")] float ghostingMultiplier = 5f;
    [SerializeField, Range(0f, 10f)][Tooltip("Gravity scale when against a wall but not on the ground")] float wallSlideMultiplier = 2f;

    [Header("Calculations")]
    Vector2 direction;
    float maxSpeedChange;
    float acceleration;
    float jumpSpeed;
    float defaultGravityScale = 1;
    Vector2 desiredVelocity;
    Vector2 moveVelocity;
    Vector2 velocity;

    [Header("Current State")]
    public bool isJumping;
    [SerializeField] bool desiredJump;
    bool ghosting;
    bool onGround;
    bool onWallLeft;
    bool onWallRight;
    [SerializeField] float coyoteTimeTimer;
    float jumpBufferTimer;

    void Awake()
    {
        //Find the player's Rigidbody and ground detection scripts
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<PlayerGround>(); 
        dash = GetComponent<PlayerAttack>();
        ghost = GetComponent<PlayerToggleCollision>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Sets the desired velocity based on the player's input and character max speed
        desiredVelocity = new Vector2(direction.x, direction.y) * Mathf.Max(maxSpeed, 0f);

        //Tracks when the player starts falling without jumping and increments the coyote time window
        if (!isJumping && !onGround)
            coyoteTimeTimer += Time.deltaTime;
        else
            coyoteTimeTimer = 0;

        //Adds a slight buffer before the jump input is ignored
        if (desiredJump && (!onGround || !onWallLeft || !onWallRight))
        {
            jumpBufferTimer += Time.deltaTime;

            if (jumpBufferTimer > jumpBuffer)
            {
                desiredJump = false;
                jumpBufferTimer = 0;
            }
        }
    }

    void FixedUpdate()
    {
        //Checks if the player is grounded, on a wall, or toggling their collision
        onGround = ground.GetOnGround();
        onWallLeft = ground.GetOnWallLeft();
        onWallRight = ground.GetOnWallRight();
        ghosting = ghost.GetGhosting();

        HandleMovement();

        velocity = body.velocity;

        //Resets multi-jumps and jumping boolean
        if (onGround)
        {
            isJumping = false;
        }

        //Checks if player is grounded or on a wall when attempting to jump
        if (desiredJump)
        {
            isJumping = true;
            DoAJump();
        }

        ModifyGravity();

        body.velocity = velocity;
    }

    void HandleMovement()
    {
        if (!dash.isDashing)
        {
            //Checks if the player is grounded
            onGround = ground.GetOnGround();
            moveVelocity = body.velocity;

            if (direction.x < 0)
                sr.flipX = true;
            if (direction.x > 0)
                sr.flipX = false;

            //Sets the acceleration based on the player's grounded status
            //Calculates the player's speed and changes the velocity accordingly
            if (ghosting)
                acceleration = maxAcceleration;
            else
                acceleration = onGround ? maxAcceleration : maxAirAcceleration;

            maxSpeedChange = acceleration * Time.deltaTime;
            moveVelocity.x = Mathf.MoveTowards(moveVelocity.x, desiredVelocity.x, maxSpeedChange);

            if (ghosting && desiredVelocity.y != 0)
                moveVelocity.y = Mathf.MoveTowards(moveVelocity.y, desiredVelocity.y, maxSpeedChange);
            else 
                moveVelocity.y = body.velocity.y;

            //Applies the velocity to the player's rigidbody
            body.velocity = moveVelocity;
        }
    }

    void DoAJump()
    {
        if (onGround || onWallLeft || onWallRight || (coyoteTimeTimer > 0.03f && coyoteTimeTimer < coyoteTime))
        {
            desiredJump = false;
            jumpBufferTimer = 0;

            //Calculates jump speed based on gravity and desired height for the jump
            jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);

            //Ensures the jump speed never ends up negative
            if (velocity.y > 0f)
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            else if (velocity.y < 0f)
                jumpSpeed += Mathf.Abs(body.velocity.y);

            //Adds the jump speed to the player's velocity
            velocity.y += jumpSpeed;

            if (!onGround && (onWallLeft || onWallRight))
                velocity.x += onWallLeft ? wallJumpSpeed : -wallJumpSpeed;
        }
    }

    void ModifyGravity()
    {
        if (ghosting)
        {
            body.gravityScale = ghostingMultiplier;
            if (direction.y < 0f)
                body.gravityScale = downwardMovementMultiplier;
        }
        //Otherwise changes gravity scale based on current phase of the jump
        else if (body.velocity.y > 0)
            body.gravityScale = upwardMovementMultiplier;
        else if (body.velocity.y < 0)
        {
            if (onWallLeft || onWallRight)
                body.gravityScale = wallSlideMultiplier;
            else
                body.gravityScale = downwardMovementMultiplier;
        }
        else if (body.velocity.y == 0)
            body.gravityScale = defaultGravityScale;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        //We find out which direction the player is inputting, and store it in direction variable
        direction = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Tracks player input of the jump button
        if (context.started)
            desiredJump = true;
    }
}
