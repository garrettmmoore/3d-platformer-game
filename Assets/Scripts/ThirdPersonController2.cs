using UnityEngine;

public class ThirdPersonController2 : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float jumpButtonGracePeriod;

    [SerializeField]
    private float jumpHorizontalSpeed;

    [SerializeField]
    private float fallDistance;

    private Animator _animator;
    private CharacterController _characterController;
    private float _ySpeed;
    private float _originalStepOffset;
    private float? _lastGroundedTime;
    private float? _jumpButtonPressedTime;
    private bool _isJumping;
    private bool _isGrounded;

    // Animator parameters
    private static readonly int InputMagnitude = Animator.StringToHash("InputMagnitude");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _originalStepOffset = _characterController.stepOffset;
    }

    private void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        
        // Determine the direction of movement along the x and z axis
        var movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        // Reflect the movement of the thumbstick (how far it's tilted) and ensure value doesn't exceed 1
        var inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        // Update the animation transition based on movement of the thumbstick
        _animator.SetFloat(InputMagnitude, inputMagnitude, 0.05f, Time.deltaTime);

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;

        // We normalize the vector as a direction vector should have a magnitude of 1
        movementDirection.Normalize();

        _ySpeed += Physics.gravity.y * Time.deltaTime;

        HandleCharacterJump();

        // Turn/rotate the character to face the direction it's moving
        HandleCharacterRotation(movementDirection);

        if (_isGrounded == false)
        {
            // Check if player is falling
            var isFallingFromPlatform = CheckIfFalling();

            // We don't want this to happen when player is going down hill or stairs
            if (_ySpeed < 0 && isFallingFromPlatform)
            {
                _animator.SetBool(IsFalling, true);
            }

            Vector3 velocity = movementDirection * (inputMagnitude * jumpHorizontalSpeed);
            velocity.y = _ySpeed;
            _characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void HandleCharacterJump()
    {
        if (_characterController.isGrounded)
        {
            _lastGroundedTime = Time.time;
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            _jumpButtonPressedTime = Time.time;
        }

        // The time that has passed since character has been on the ground
        var hasGroundedRecently = Time.time - _lastGroundedTime <= jumpButtonGracePeriod;
        
        // The time that has passed since the jump button has been pressed
        var hasJumpedRecently = Time.time - _jumpButtonPressedTime <= jumpButtonGracePeriod;

        // Check if player has been grounded recently
        if (hasGroundedRecently)
        {
            _characterController.stepOffset = _originalStepOffset;
            _ySpeed = -0.5f;

            _isGrounded = true;
            _animator.SetBool(IsGrounded, _isGrounded);

            _isJumping = false;
            _animator.SetBool(IsJumping, _isJumping);
            _animator.SetBool(IsFalling, _isJumping);

            // Check if player has jumped recently
            if (hasJumpedRecently)
            {
                _ySpeed = jumpSpeed;
                _isJumping = true;
                _animator.SetBool(IsJumping, _isJumping);
                _jumpButtonPressedTime = null;
                _lastGroundedTime = null;
            }
        }
        else
        {
            _characterController.stepOffset = 0;
            _isGrounded = false;
            _animator.SetBool(IsGrounded, _isGrounded);

            // Check if player is falling
            var isFalling = _isJumping && _ySpeed < 0;
            var isFallingFromPlatform = CheckIfFalling();

            // We don't want this to happen when player is going down hill or stairs
            if (isFalling && isFallingFromPlatform)
            {
                _animator.SetBool(IsFalling, true);
            }
        }
    }

    private void HandleCharacterRotation(Vector3 movementDirection)
    {
        if (movementDirection != Vector3.zero)
        {
            _animator.SetBool(IsMoving, true);

            // Smoothly change the rotation of the character
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            var maxDegrees = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, maxDegrees);
        }
        else
        {
            _animator.SetBool(IsMoving, false);
        }
    }

    private bool CheckIfFalling()
    {
        Transform transform1 = transform;
        Vector3 position = transform1.position;
        Vector3 up = transform1.up;
        var ray = new Ray(position, -up);
        Debug.DrawRay(position, -up, Color.green);
        return !Physics.Raycast(ray, out RaycastHit hit, fallDistance) && _ySpeed <= 0;
    }

    private void OnAnimatorMove()
    {
        if (_isGrounded)
        {
            Vector3 velocity = _animator.deltaPosition;
            velocity.y = _ySpeed * Time.deltaTime;
            _characterController.Move(velocity);
        }
    }

    /// <summary>
    /// Hide the mouse cursor when the application gets focused
    /// </summary>
    /// <param name="focus"></param>
    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}