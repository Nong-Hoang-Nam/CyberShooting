using UnityEngine;
using UnityEngine.InputSystem;

public class AutomaticShooting : MonoBehaviour
{
    [SerializeField] private InputActionReference _shootAction;
    [SerializeField] private float _cooldown = 0.1f; // Thời gian cooldown mặc định
    [SerializeField] private GunRaycasting _gunRaycasting; // Tham chiếu đến GunRaycasting

    private float _lastShotTime;

    void Start()
    {
        if (_gunRaycasting == null)
        {
            Debug.LogError("GunRaycasting not assigned to AutomaticShooting!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (_shootAction.action.IsPressed() && FinishCooldown() && _gunRaycasting.CanShoot())
        {
            _gunRaycasting.Shoot(); // Gọi hàm Shoot từ GunRaycasting
            _lastShotTime = Time.time;
        }
    }

    private bool FinishCooldown() => Time.time - _lastShotTime >= _cooldown;

    // Tùy chọn: Thêm event OnShoot nếu cần
    public void OnShoot()
    {
        if (_gunRaycasting != null && _gunRaycasting.CanShoot())
        {
            _gunRaycasting.Shoot();
        }
    }
}