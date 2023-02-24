using UnityEngine;

public class ThirdPersonAnimation : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("speed");
    private Animator _animator;
    private Rigidbody _rb;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        var movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        var speed = Mathf.Clamp01(movementDirection.magnitude);
        _animator.SetFloat(Speed, speed, 0.05f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        Debug.Log("Animator Moved!");
        Vector3 velocity = _animator.deltaPosition;
        velocity.y = _rb.velocity.y * Time.deltaTime;
        _rb.AddForce(velocity);
    }
}