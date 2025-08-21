using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class GunRaycasting : MonoBehaviour
{
    [SerializeField] private Transform _firingPos;
    [SerializeField] private Transform _aimingCamera;
    [SerializeField] private Transform _hitMarkerPrefab;
    [SerializeField] private Transform _explosionVFXPrefab;
    [SerializeField] private AudioClip _shootSoundClip;
    [SerializeField] private AudioClip _reloadSoundClip;
    [SerializeField] private AudioClip _timeHackSoundClip;
    [SerializeField] private int _damage;
    [SerializeField] private float _range = 100f;
    [SerializeField] private int _maxAmmo;
    [SerializeField] private float _fireRate;
    [SerializeField] private InputActionReference _reloadAction;
    [SerializeField] private InputActionReference _timeHackAction;
    [SerializeField] private InputActionReference _aimAction;
    [SerializeField] private InputActionReference _switchWeaponAction;
    [SerializeField] private float _timeHackDuration = 3f;
    [SerializeField] private float _timeHackCooldown = 10f;
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _aimFOV = 30f;
    [SerializeField] private Vector3 _normalCameraOffset = new Vector3(0, 1.5f, -2f);
    [SerializeField] private Vector3 _aimCameraOffset = new Vector3(0.2f, 1.2f, -1f);

    private int _currentAmmo;
    private float _fireTimer;
    private float _timeHackTimer;
    private Camera _cameraComponent;
    private bool _isAiming;
    private AudioSource _audioSource;
    private int _currentWeaponIndex = 0;
    [SerializeField] private Transform[] _weapons;
    private string _weaponType = "Pistol";
    [SerializeField] private TMPro.TextMeshProUGUI _ammoText;
    [SerializeField] private TMPro.TextMeshProUGUI _weaponText;
    [SerializeField] private Image[] crosshairImages; // Danh sách crosshair cho mỗi súng

    public string[] weaponTypes = { "Pistol", "SMG", "Railgun" };

    void Start()
    {
        _currentAmmo = _maxAmmo;
        _fireTimer = _fireRate;
        _timeHackTimer = _timeHackCooldown;
        _cameraComponent = _aimingCamera.GetComponentInChildren<Camera>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        if (_weapons == null || _weapons.Length == 0 || _weapons.All(w => w == null))
        {
            Debug.LogError("No weapons assigned. Please assign Pistol, SMG, and Railgun in the Inspector.");
            enabled = false;
            return;
        }
        // Tự động tìm crosshair từ Canvas của mỗi súng
        List<Image> foundCrosshairs = new List<Image>();
        foreach (var weapon in _weapons)
        {
            if (weapon != null)
            {
                Canvas canvas = weapon.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    Image crosshair = canvas.GetComponentInChildren<Image>();
                    if (crosshair != null)
                    {
                        foundCrosshairs.Add(crosshair);
                        Debug.Log("Found crosshair for weapon: " + weapon.name);
                    }
                }
            }
        }
        crosshairImages = foundCrosshairs.ToArray();
        SwitchWeapon(0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (_cameraComponent == null)
        {
            Debug.LogError("No Camera found in or under " + _aimingCamera.name);
            enabled = false;
            return;
        }
        ShowAllCrosshairs();
        UpdateUI();
    }

    void Update()
    {
        _fireTimer += Time.deltaTime;
        if (_reloadAction.action.triggered && _currentAmmo < _maxAmmo)
        {
            Reload();
        }
        if (_timeHackAction.action.triggered && _timeHackTimer >= _timeHackCooldown)
        {
            ActivateTimeHack();
        }
        if (_switchWeaponAction.action.triggered)
        {
            SwitchWeapon((_currentWeaponIndex + 1) % 3);
        }
        _timeHackTimer += Time.deltaTime;

        _isAiming = _aimAction.action.IsPressed();
        UpdateAim();
        UpdateUI();
    }

    public void Shoot()
    {
        if (_currentAmmo <= 0 || _fireTimer < _fireRate) return;

        Camera cam = _cameraComponent;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out var raycastHit, _range))
        {
            Instantiate(_hitMarkerPrefab, raycastHit.point, Quaternion.LookRotation(raycastHit.normal), parent: raycastHit.collider.transform);
            CreateExplosionEffect(raycastHit.point, raycastHit.normal);
            if (raycastHit.collider.TryGetComponent<DroneAI>(out var drone))
            {
                drone.TakeDamage(_damage);
            }
            else if (raycastHit.collider.TryGetComponent<SmallDroneAI>(out var smallDrone))
            {
                smallDrone.TakeDamage(_damage);
            }
            else if (raycastHit.collider.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(_damage);
            }
        }

        _audioSource.PlayOneShot(_shootSoundClip);
        ReduceAmmo();
        _fireTimer = 0f;
    }

    private void Reload()
    {
        _audioSource.PlayOneShot(_reloadSoundClip);
        _currentAmmo = _maxAmmo;
    }

    private void ActivateTimeHack()
    {
        _audioSource.PlayOneShot(_timeHackSoundClip);
        Time.timeScale = 0.3f;
        _timeHackTimer = 0f;
        Invoke("EndTimeHack", _timeHackDuration);
    }

    private void EndTimeHack()
    {
        Time.timeScale = 1f;
    }

    private void UpdateAim()
    {
        if (_cameraComponent == null) return;

        if (_isAiming)
        {
            _cameraComponent.fieldOfView = _aimFOV;
            _aimingCamera.localPosition = _aimCameraOffset;
            ShowAllCrosshairs();
        }
        else
        {
            _cameraComponent.fieldOfView = _normalFOV;
            _aimingCamera.localPosition = _normalCameraOffset;
            ShowAllCrosshairs();
        }
    }

    private void SwitchWeapon(int index)
    {
        _currentWeaponIndex = index;
        _weaponType = weaponTypes[index];
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] != null) _weapons[i].gameObject.SetActive(i == _currentWeaponIndex);
        }
        UpdateWeaponStats();
        UpdateUI();
    }

    private void UpdateWeaponStats()
    {
        if (_weaponType == "Railgun")
        {
            _damage = 20;
            _maxAmmo = 10;
            _fireRate = 1f;
        }
        else if (_weaponType == "SMG")
        {
            _damage = 5;
            _maxAmmo = 60;
            _fireRate = 0.1f;
        }
        else // Pistol
        {
            _damage = 10;
            _maxAmmo = 15;
            _fireRate = 0.5f;
        }
        _currentAmmo = _maxAmmo;
    }

    private void UpdateUI()
    {
        if (_ammoText != null) _ammoText.text = $"Đạn: {_currentAmmo}/{_maxAmmo}";
        if (_weaponText != null) _weaponText.text = $"Vũ khí: {_weaponType}";
    }

    private void CreateExplosionEffect(Vector3 position, Vector3 normal)
    {
        if (_explosionVFXPrefab != null)
        {
            Transform explosion = Instantiate(_explosionVFXPrefab, position, Quaternion.LookRotation(normal));
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null && !ps.main.loop)
            {
                Destroy(explosion.gameObject, ps.main.duration);
            }
        }
    }

    public bool CanShoot() => _currentAmmo > 0 && _fireTimer >= _fireRate;
    private void ReduceAmmo() => _currentAmmo--;

    public void HideAllCrosshairs()
    {
        foreach (var crosshair in crosshairImages)
        {
            if (crosshair != null) crosshair.enabled = false;
        }
        Debug.Log("All crosshairs hidden");
    }

    public void ShowAllCrosshairs()
    {
        foreach (var crosshair in crosshairImages)
        {
            if (crosshair != null) crosshair.enabled = true;
        }
        Debug.Log("All crosshairs shown");
    }
}