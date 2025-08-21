using UnityEngine;

public class ElectricTrap : MonoBehaviour
{
    [Header("Stats")]
    public float damage = 15f;
    public float stunDuration = 2f;
    [Header("Effects")]
    public AudioClip zapSound;
    public ParticleSystem electricWallPS;

    private bool isActive = true;

    void Start()
    {
        if (electricWallPS != null)
        {
            electricWallPS.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player hit by ElectricTrap, damage: " + damage);
            }
            PlayEffects();
        }
        else if (other.CompareTag("Enemy"))
        {
            DroneAI drone = other.GetComponent<DroneAI>();
            if (drone != null)
            {
                drone.Stun(stunDuration);
                Debug.Log("Drone stunned for " + stunDuration + " seconds");
            }
            SmallDroneAI smallDrone = other.GetComponent<SmallDroneAI>();
            if (smallDrone != null)
            {
                smallDrone.Stun(stunDuration);
                Debug.Log("SmallDrone stunned for " + stunDuration + " seconds");
            }
            PlayEffects();
        }
    }

    private void PlayEffects()
    {
        if (zapSound != null)
        {
            AudioSource.PlayClipAtPoint(zapSound, transform.position);
        }
        if (electricWallPS != null)
        {
            electricWallPS.Emit(20); // Tăng hiệu ứng khi kích hoạt
        }
    }

    public void DeactivateTrap()
    {
        isActive = false;
        if (electricWallPS != null) electricWallPS.Stop();
    }
}