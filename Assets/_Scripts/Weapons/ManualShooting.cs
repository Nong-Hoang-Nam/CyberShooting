using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ManualShooting : Shooting
{
    [SerializeField] private InputActionReference _shootAction;

    public UnityEvent OnShoot;

    private void Update()
    {
        if (_shootAction.action.triggered)
        {
            Shoot();
        }
    }

    private void Shoot() => OnShoot.Invoke();
}
