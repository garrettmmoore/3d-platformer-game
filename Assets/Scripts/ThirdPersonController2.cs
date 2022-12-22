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

        var movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        var inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            inputMagnitude /= 2;
        }

        _animator.SetFloat(InputMagnitude, inputMagnitude, 0.05f, Time.deltaTime);

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        _ySpeed += Physics.gravity.y * Time.deltaTime;

        if (_characterController.isGrounded)
        {
            _lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            _jumpButtonPressedTime = Time.time;
        }

        // Check if player has been grounded recently
        if (Time.time - _lastGroundedTime <= jumpButtonGracePeriod)
        {
            _characterController.stepOffset = _originalStepOffset;
            _ySpeed = -0.5f;
            _animator.SetBool(IsGrounded, true);
            _isGrounded = true;
            _animator.SetBool(IsJumping, false);
            _isJumping = false;
            _animator.SetBool(IsFalling, false);

            // Check if player has jumped recently
            if (Time.time - _jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                _ySpeed = jumpSpeed;
                _animator.SetBool(IsJumping, true);
                _isJumping = true;
                _jumpButtonPressedTime = null;
                _lastGroundedTime = null;
            }
        }
        else
        {
            _characterController.stepOffset = 0;
            _animator.SetBool(IsGrounded, false);
            _isGrounded = false;

            // Check if player is falling
            var isFalling = _isJumping && _ySpeed < 0;
            var isFallingFromPlatform = CheckIfFalling();

            // We don't want this to happen when player is going down hill or stairs
            // if (isFalling && isFallingFromPlatform)
            if (isFalling && isFallingFromPlatform)
            {
                _animator.SetBool(IsFalling, true);
            }
        }

        if (movementDirection != Vector3.zero)
        {
            _animator.SetBool(IsMoving, true);

            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                toRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            _animator.SetBool(IsMoving, false);
        }

        if (_isGrounded == false)
        {
            // Check if player is falling
            // var isFalling = _isJumping && _ySpeed < 0;
            var isFallingFromPlatform = CheckIfFalling();

            // We don't want this to happen when player is going down hill or stairs
            // if (isFalling && isFallingFromPlatform)
            if (_ySpeed < 0 && isFallingFromPlatform)
            {
                _animator.SetBool(IsFalling, true);
            }

            Vector3 velocity = movementDirection * (inputMagnitude * jumpHorizontalSpeed);
            velocity.y = _ySpeed;

            _characterController.Move(velocity * Time.deltaTime);
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

    private void OnApplicationFocus(bool focus)
    {
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}