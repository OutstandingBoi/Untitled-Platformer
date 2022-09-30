using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Components")]
    GameManager gameManager;

    [Header("Stats")]
    [SerializeField] [Tooltip("Max health of the object")] float maxHealth = 100f;
    [Tooltip("Current health of the object")] public float currentHealth;
    [Tooltip("Whether the object has health above 0")] public bool alive { get { return currentHealth > 0f; } }
    [SerializeField, Range(0f, 1f)] [Tooltip("How long in seconds the game freezes when this object is hit")] float hitStopTime;

    [Header("State")]
    [SerializeField] bool isEnemy;
    [SerializeField] bool isPlayer;
    bool waiting;

    void Awake()
    {
        currentHealth = maxHealth;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void TakeDamage(float amount)
    {
        StartCoroutine(HitStop());
        currentHealth -= amount;

        if (!alive)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //Removes the object from the game manager's list of enemies if it is labeled as an enemy
        if (isEnemy) gameManager.RemoveEnemy(gameObject);

        //Disables player specific components to prevent errors
        if (isPlayer)
        {
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerToggleCollision>().enabled = false;
        }

        //Gets components of the object
        var collider = GetComponent<Collider2D>();
        var sr = GetComponent<SpriteRenderer>();
        var deathFX = GetComponent<ParticleSystem>();

        //Disables the sprite renderer and collider
        sr.enabled = false;
        collider.enabled = false;

        //Only attempts to trigger death particles if the object has a particle system
        if (deathFX != null)
        {
            deathFX.Play();
            yield return new WaitForSeconds(deathFX.main.duration);
        }

        //Destroys the game object
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    IEnumerator HitStop()
    {
        if (!waiting)
        {
            //Pauses the game when the object is hit to give attacks impact
            waiting = true;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitStopTime);
            Time.timeScale = 1f;
            waiting = false;
        }
    }

    public float MaxHealth { get { return maxHealth; } }
}
