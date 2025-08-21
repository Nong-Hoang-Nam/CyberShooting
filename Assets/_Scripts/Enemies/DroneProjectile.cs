using UnityEngine;

public class DroneProjectile : MonoBehaviour
{
    public float damage = 5f;
    public float lifetime = 5f;
    public GameObject explosionVFX;
    public AudioClip shootSound;
    public AudioClip explosionSound;

    private AudioSource audioSource;
    private bool isInitialized = false; // Delay khởi tạo

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (shootSound != null) audioSource.PlayOneShot(shootSound);
        Invoke("SetInitialized", 0.1f); // Delay 0.1s để tránh va chạm ngay lập tức
        Destroy(gameObject, lifetime);
    }

    void SetInitialized()
    {
        isInitialized = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isInitialized) return; // Tránh va chạm trong giai đoạn khởi tạo

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Explode();
        }
        else if (other.gameObject != transform.root.gameObject) // Tránh va chạm với parent hoặc bản thân
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }
        Destroy(gameObject);
    }
}