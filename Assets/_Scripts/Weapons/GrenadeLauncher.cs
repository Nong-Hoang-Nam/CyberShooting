using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    [SerializeField] private Rigidbody _bulletPrefab;
    [SerializeField] private Transform _firingPos;
    [SerializeField] private float _bulletSpeed;

    public void LaunchGrenade()
    {
        var bullet = Instantiate(_bulletPrefab, _firingPos.position, _firingPos.rotation);
        bullet.linearVelocity = _firingPos.forward * _bulletSpeed;
        Destroy(bullet.gameObject, 10f);
    }
}
    