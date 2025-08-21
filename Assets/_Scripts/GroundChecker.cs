using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private MeshRenderer _renderer;

    void Update()
    {
        _renderer.enabled = _controller.isGrounded;
    }
}
