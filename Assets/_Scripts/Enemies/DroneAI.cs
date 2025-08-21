using UnityEngine;

public class DroneAI : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float attackRange = 15f;
    public float attackDamage = 10f;
    [Header("Projectile")]
    public GameObject projectilePrefab;
    [Range(0.2f, 10f)] public float fireInterval = 2f;
    public float projectileSpeed = 10f;
    [Header("Prefabs and Effects")]
    public GameObject smallDronePrefab;
    public ParticleSystem explosionEffect;
    [Header("Audio")]
    public AudioSource deathSound;
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float hoverHeight = 5f;

    private Transform player;
    private GameManager gameManager;
    private float fireTimer = 0f;
    private bool isStunned = false;
    private float stunTimer = 0f;
    private Rigidbody rb;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on " + gameObject.name + ". Adding one now.");
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.freezeRotation = true;
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gameManager = FindObjectOfType<GameManager>();
        if (player == null) Debug.LogError("Player not found with tag 'Player'!");
        gameObject.tag = "Enemy";
        Debug.Log(gameObject.name + " initialized with health: " + health + ", fireInterval: " + fireInterval);
    }

    void Update()
    {
        if (player == null || isStunned)
        {
            fireTimer = 0f;
            return;
        }

        // Luôn đuổi theo player
        Vector3 targetPosition = new Vector3(player.position.x, hoverHeight, player.position.z);
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireInterval)
            {
                ShootProjectile();
                fireTimer = 0f;
                Debug.Log(gameObject.name + " fired, next shot in: " + fireInterval + " seconds");
            }
        }
        else
        {
            fireTimer = 0f;
        }

        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                Debug.Log(gameObject.name + " stun ended");
            }
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 shootDirection = (player.position - transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(shootDirection));
            projectile.transform.localScale = new Vector3(1f, 1f, 1f);
            Rigidbody projRb = projectile.GetComponent<Rigidbody>();
            if (projRb != null) projRb.linearVelocity = shootDirection * projectileSpeed;
            Debug.Log(gameObject.name + " fired projectile with scale: " + projectile.transform.localScale);
        }
    }

    public void Stun(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            stunTimer = duration;
            Debug.Log(gameObject.name + " stunned for " + duration + " seconds");
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        Debug.Log(gameObject.name + " taking damage, health: " + health + ", damage: " + amount);
        health -= amount;
        if (health <= 0)
        {
            isDead = true;
            if (explosionEffect != null) Instantiate(explosionEffect, transform.position, Quaternion.identity);
            if (deathSound != null) deathSound.Play();
            if (smallDronePrefab != null)
            {
                int smallDroneCount = Random.value > 0.7f ? 4 : 2;
                for (int i = 0; i < smallDroneCount; i++)
                {
                    GameObject smallDrone = Instantiate(smallDronePrefab, transform.position + Random.insideUnitSphere * 1f, Quaternion.identity);
                    smallDrone.transform.localScale = Vector3.one * 0.5f;
                }
            }
            if (gameManager != null)
            {
                gameManager.EnemyKilled(gameObject);
            }
            Destroy(gameObject, deathSound != null && deathSound.clip != null ? deathSound.clip.length : 0f);
        }
    }
}