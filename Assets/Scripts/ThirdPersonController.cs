using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    // Input fields
    private ThirdPersonActionsAsset _playerActionsAsset;
    private InputAction _move;

    // Movement fields
    [SerializeField] private float movementForce = 1f;
    [SerializeField] private float jumpForce = 7f;
    // [SerializeField] private float maxSpeed = 5f;
    private Rigidbody _rb;
    private Vector3 _forceDirection = Vector3.zero;
    // private float ySpeed;
    
    
    // Camera fields
    [SerializeField] private Camera playerCamera;
    
    // Animator fields
    private Animator _animator;
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Attack = Animator.StringToHash("attack");


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _playerActionsAsset = new ThirdPersonActionsAsset();
    }

    private void OnEnable()
    {
        // Get a reference to our player movement
        _move = _playerActionsAsset.Player.Move;

        // Subscribe to our Jump input event
        _playerActionsAsset.Player.Jump.started += DoJump;

        // Subscribe to our Attack input event
        _playerActionsAsset.Player.Attack.started += DoAttack;
        
        // Turn on our action map
        _playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        // UnSubscribe from our Jump input event
        _playerActionsAsset.Player.Jump.started -= DoJump;

        // // Unsubscribe from our Attack input event
        _playerActionsAsset.Player.Attack.started -= DoAttack;

        // Turn off our action map
        _playerActionsAsset.Player.Disable();
    }

    private void FixedUpdate()
    {
        // We want our motion to be relative to the camera
        _forceDirection += GetCameraRight(playerCamera) * (_move.ReadValue<Vector2>().x * movementForce);
        _forceDirection += GetCameraForward(playerCamera) * (_move.ReadValue<Vector2>().y * movementForce);

        // Apply the force to our rigid body
        _rb.AddForce(_forceDirection, ForceMode.Impulse);
        
        // When we let go of gamepad, the character won't continue to accelerate
        _forceDirection = Vector3.zero;

        // ySpeed += Physics.gravity.y * Time.fixedDeltaTime;
        // We get rid of "floaty-ness" by bringing the character down to the ground faster
        // We are increasing the acceleration as we fall
        if (_rb.velocity.y < 0f)
        {
            _rb.velocity -= Vector3.down * (Physics.gravity.y * Time.fixedDeltaTime);
        }

        // Here we put a cap on our horizontal velocity
        // Vector3 horizontalVelocity = _rb.velocity;
        
        // Remove the vertical component so we only have horizontal velocity
        // horizontalVelocity.y = 0;
        
        
        // if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        // {
            // We add back in the vertical component so we don't stop or float in midair
            // _rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * _rb.velocity.y;
        // }

        LookAt();
    }

    private void LookAt()
    {
        Vector3 direction = _rb.velocity;
        // We don't want player to look up or down, we just want them to rotate on the vertical axis
        direction.y = 0f;
    
        // If there is some input from the player and we are moving, then change the direction that we look
        if (_move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            _rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        // Prevent our player from rotating automatically from Unity's physics engine if we are not getting any input
        else
        {
            _rb.angularVelocity = Vector3.zero;
        }
    }

    // Takes the forward/right directions and projects them onto the horizontal plane
    // This is necessary b/c our camera is sometimes tilted
    private Vector3 GetCameraForward(Camera playerCam)
    {
        Vector3 forward = playerCam.transform.forward;
        forward.y = 0;
        
        // We normalize b/c when we take off the vertical component, the length of it is no longer
        // And we don't want our speed/movement to depend on the angle of the camera
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCam)
    {
        Vector3 right = playerCam.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        // We only want to jump if we are grounded
        if (IsGrounded())
        {
            _animator.SetTrigger(Jump);
            _forceDirection += Vector3.up * jumpForce;
        }
    }

    private bool IsGrounded()
    {
        // Use ray casting to check if there is something directly below us
        var ray = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down);
        return Physics.Raycast(ray, out RaycastHit _, 0.3f);
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        _animator.SetTrigger(Attack);
    }
}