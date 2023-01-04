using System;
using UnityEngine;

public class ThirdPersonController2 : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float gravityMultiplier;

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
    private bool _isSliding;
    private Vector3 _slopeSlideVelocity;

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

        // Implement gravity to make the jump less floaty
        var gravity = Physics.gravity.y * gravityMultiplier;

        // Increase the gravity if the jump button is released
        if (_isJumping && _ySpeed > 0 && Input.GetButton("Jump") == false)
        {
            gravity *= 2;
        }

        _ySpeed += gravity * Time.deltaTime;
        SetSlopeSlideVelocity();

        if (_slopeSlideVelocity == Vector3.zero)
        {
            _isSliding = false;
        }

        HandleCharacterJump(gravity);

        // Turn/rotate the character to face the direction it's moving
        HandleCharacterRotation(movementDirection);

        if (_isGrounded == false && _isSliding == false)
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

        if (_isSliding)
        {
            // make sure the character is still pushing down into the ground
            Vector3 velocity = _slopeSlideVelocity;
            velocity.y = _ySpeed;
            
            // make sure character moves at the same speed regardless of the framerate
            _characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void HandleCharacterJump(float gravity)
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
        var hasJumpedRecently = Time.time - _jumpButtonPressedTime <= jumpButtonGracePeriod && _isSliding == false;

        // Check if player has been grounded recently
        if (hasGroundedRecently)
        {
            if (_slopeSlideVelocity != Vector3.zero)
            {
                _isSliding = true;
            }
            
            _characterController.stepOffset = _originalStepOffset;
            if (_isSliding == false)
            {
                _ySpeed = -0.5f;
            }

            _isGrounded = true;
            _animator.SetBool(IsGrounded, _isGrounded);

            _isJumping = false;
            _animator.SetBool(IsJumping, _isJumping);
            _animator.SetBool(IsFalling, _isJumping);

            // Check if player has jumped recently
            if (hasJumpedRecently)
            {
                // Takes the height and gravity value and returns the required speed needed
                // to reach the desired height
                var requiredSpeed = Mathf.Sqrt(jumpHeight * -3 * gravity);
                _ySpeed = requiredSpeed;

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
        if (_isGrounded && _isSliding == false)
        {
            Vector3 velocity = _animator.deltaPosition;
            velocity.y = _ySpeed * Time.deltaTime;
            _characterController.Move(velocity);
        }
    }

    /// <summary>
    /// Set the velocity of the slide when a character slides down a slope
    /// </summary>
    private void SetSlopeSlideVelocity()
    {
        // Check if there is ground underneath the character via raycast
        var groundExists = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 5);

        // We only want to proceed if the raycast hits the ground
        if (groundExists)
        {
            // Vector coming out of the ground's surface
            Vector3 groundVector = hitInfo.normal;
            
            // Get the angle of the slope
            var angle = Vector3.Angle(groundVector, Vector3.up);

            // Check if the angle of the current slope is greater than the slope limit
            if (angle >= _characterController.slopeLimit)
            {
                var downwardCharacterVelocity = new Vector3(0, _ySpeed, 0);
                
                // Slide the character downwards based on the direction the character is facing
                _slopeSlideVelocity = Vector3.ProjectOnPlane(downwardCharacterVelocity, groundVector);
                return;
            }
        }

        if (_isSliding)
        {
            // Bring the slide to a gradual stop
            _slopeSlideVelocity -= _slopeSlideVelocity * (Time.deltaTime * 3);
            if (_slopeSlideVelocity.magnitude > 1)
            {
                return;
            }
        }
        
        // If character is not on any ground isn't steep enough to require a slide, set to zero
        _slopeSlideVelocity = Vector3.zero;
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