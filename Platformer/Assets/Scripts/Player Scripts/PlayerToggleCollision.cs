using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerToggleCollision : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerWallDetection wallDetection;
    Health health;
    SpriteRenderer sr;
    new CircleCollider2D collider;

    [Header("Stats")]
    [SerializeField, Range(0f, 100f)] [Tooltip("The player's maximum resource to toggle collision")] float maxEnergy;
    [SerializeField] [Tooltip("The player's current resource to toggle collision")] float currentEnergy;
    [SerializeField, Range(0f, 100f)] [Tooltip("How fast the player drains their energy")] float energyDrain;
    [SerializeField, Range(0f, 100f)] [Tooltip("How fast the player regains their energy")] float energyRegen;

    [Header("Current State")]
    bool goingGhost = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        health = GetComponent<Health>();
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        HandleGhosting();
    }

    private void HandleGhosting()
    {
        //Changes the player's layer depending on their input and drains or regenerates their energy accordingly
        if ((goingGhost || wallDetection.GetInWall()) && currentEnergy > 0)
        {
            collider.radius = 0.22f;
            gameObject.layer = LayerMask.NameToLayer("Ghost");
            sr.color = new Color(1f, 1f, 1f, 0.5f);
            currentEnergy -= energyDrain * Time.deltaTime;
        }
        else if (wallDetection.GetInWall() && currentEnergy <= 0)
        {
            health.TakeDamage(health.MaxHealth);
        }
        else
        {
            collider.radius = 0.495f;
            gameObject.layer = LayerMask.NameToLayer("Player");
            sr.color = new Color(1f, 1f, 1f, 1f);
            currentEnergy += energyRegen * Time.deltaTime;
        }

        //Keeps the player's energy from going over their maximum or under 0
        if (currentEnergy >= maxEnergy)
            currentEnergy = maxEnergy;

        if (currentEnergy <= 0)
            currentEnergy = 0;
    }

    public void OnGhost(InputAction.CallbackContext context)
    {
        //Checks the player's input, if they have enough energy, and if they are toggled to have a ghost dash
        if (context.ReadValue<float>() != 0 && currentEnergy > energyDrain)
            goingGhost = true;
        else
            goingGhost = false;
    }

    public bool GetGhosting() { return goingGhost; }
    public float MaxEnergy { get { return maxEnergy; } }
    public float CurrentEnergy { get { return currentEnergy; } }
}
