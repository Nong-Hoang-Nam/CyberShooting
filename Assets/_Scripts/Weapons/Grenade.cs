using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionForce;

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        PushNearbyObjects();
        Destroy(gameObject);
    }

    private void PushNearbyObjects()
    {
        var victims = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach(var victim in victims)
        {
            if (victim.TryGetComponent<Rigidbody>(out var rigid))
            {
                rigid.AddExplosionForce(_explosionForce, transform.position, _explosionRadius,
                    1f, ForceMode.Impulse);
            }

            if (victim.TryGetComponent<ExplosibleCube>(out var cube))
            {
                cube.Explode(_explosionForce, transform.position, _explosionRadius);
            }
        }
    }
}
