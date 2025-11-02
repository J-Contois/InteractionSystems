using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

namespace Player
{
    /// <summary>
    /// Handles player movement, camera rotation, sprint, crouch, prone, stamina, and camera bobbing.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Motion settings")]
        
        [Tooltip("Player movement speed in units per second")]
        [SerializeField] private float speed = 5f;
        
        [Tooltip("Sprint speed in units per second")]
        [SerializeField] private float sprintSpeed = 9f;
        
        [Tooltip("Crouch speed in units per second")]
        [SerializeField] private float crouchSpeed = 2.5f;
        
        [Tooltip("Prone speed in units per second")]
        [SerializeField] private float proneSpeed = 1.2f;
        
        [Tooltip("Camera rotation speed in degrees per second")]
        [SerializeField] private float rotationSpeed = 5f;
        
        [Tooltip("Reference to the player's Rigidbody")]
        [SerializeField] private Rigidbody rigidBody;
        
        [Tooltip("Vertical rotation limits (pitch): X = minimum, Y = maximum")]
        [SerializeField] private Vector2 minMaxYaw = new(-90f, 90f);
        
        [Tooltip("Movement speed while aiming")]
        [SerializeField] private float aimingSpeed = 2f;
        
        [Header("Transforms References")]
        
        [Tooltip("Transform player root (horizontal rotation)")]
        [SerializeField] private Transform root;
        
        [Tooltip("Head/camera transform (vertical rotation)")]
        [SerializeField] private Transform head;
        
        [Header("Weapon Reference")]
        
        [Tooltip("Reference to the WeaponController")]
        [SerializeField] private Weapon.WeaponController weaponController;
        
        [Header("Stamina Settings")]
        
        [Tooltip("Maximum stamina")]
        [SerializeField] private float maxStamina = 100f;
        
        [Tooltip("Stamina drain per second when sprinting")]
        [SerializeField] private float sprintStaminaDrain = 25f;
        
        [Tooltip("Stamina regeneration per second")]
        [SerializeField] private float staminaRegen = 15f;
        
        [Header("Camera Bobbing")]
        
        [Tooltip("Bobbing amplitude")]
        [SerializeField] private float bobAmplitude = 0.05f;
        
        [Tooltip("Bobbing frequency")]
        [SerializeField] private float bobFrequency = 8f;
        
        [Tooltip("Camera transform for bobbing")]
        [SerializeField] private Transform cameraTransform;
        
        [Header("Events")]
        /// <summary>
        /// Invoked when stamina changes (normalized 0-1).
        /// </summary>
        public UnityEvent<float> onStaminaChanged;
        /// <summary>
        /// Invoked when movement state changes ("Sprint", "Crouch", "Prone", "Aiming", "Normal").
        /// </summary>
        public UnityEvent<string> onMovementStateChanged;

        private Vector3 _input = Vector3.zero;
        private Vector2 _lookInput;
        private Vector2 _currentRotation;
        private float _currentStamina;
        private float _bobTimer;
        private bool _isSprinting;
        private bool _isCrouching;
        private bool _isProne;
        private bool CanSprint => _currentStamina > 0.1f && !_isCrouching && !_isProne;

        private float _defaultCameraY; // Stores the initial local Y position of the camera

        private enum MovementState { Normal, Sprint, Crouch, Prone, Aiming }
        private MovementState _state = MovementState.Normal;

        /// <summary>
        /// Automatically called in the editor to initialize references.
        /// </summary>
        private void Reset()
        {
            rigidBody = GetComponent<Rigidbody>();
        }
        /// <summary>
        /// Stores initial camera position for bobbing.
        /// </summary>
        private void Awake()
        {
            if (cameraTransform != null)
                _defaultCameraY = cameraTransform.localPosition.y;
        }
        /// <summary>
        /// Handles input, stamina, camera bobbing, and rotation.
        /// </summary>
        private void Update()
        {
            HandleInput();
            HandleStamina();
            HandleCameraBobbing();
            HandleRotation();
        }
        /// <summary>
        /// Handles smooth rotation for root and head transforms.
        /// </summary>
        private void HandleRotation()
        {
            // Smoothly interpolate rotation for both root and head
            _currentRotation.x += -_lookInput.y * rotationSpeed * Time.deltaTime;
            _currentRotation.y += _lookInput.x * rotationSpeed * Time.deltaTime;
            _currentRotation.x = Mathf.Clamp(_currentRotation.x, minMaxYaw.x, minMaxYaw.y);
            // Interpolate root rotation (horizontal)
            Quaternion targetRootRot = Quaternion.Euler(0f, _currentRotation.y, 0f);
            root.localRotation = Quaternion.Slerp(root.localRotation, targetRootRot, rotationSpeed * Time.deltaTime);
            // Interpolate head rotation (vertical)
            Quaternion targetHeadRot = Quaternion.Euler(_currentRotation.x, 0f, 0f);
            head.localRotation = Quaternion.Slerp(head.localRotation, targetHeadRot, rotationSpeed * Time.deltaTime);
        }
        /// <summary>
        /// Handles movement logic based on current state (sprint, crouch, prone, aiming, normal).
        /// </summary>
        private void FixedUpdate()
        {
            float currentSpeed;
            if (weaponController && weaponController.IsAiming)
                _state = MovementState.Aiming;
            else if (_isSprinting)
                _state = MovementState.Sprint;
            else if (_isCrouching)
                _state = MovementState.Crouch;
            else if (_isProne)
                _state = MovementState.Prone;
            else
                _state = MovementState.Normal;

            switch (_state)
            {
                case MovementState.Sprint:
                    currentSpeed = sprintSpeed;
                    break;
                case MovementState.Crouch:
                    currentSpeed = crouchSpeed;
                    break;
                case MovementState.Prone:
                    currentSpeed = proneSpeed;
                    break;
                case MovementState.Aiming:
                    currentSpeed = aimingSpeed;
                    break;
                default:
                    currentSpeed = speed;
                    break;
            }
            rigidBody.linearVelocity = root.rotation * (currentSpeed * _input.normalized);
        }
        /// <summary>
        /// Handles input for sprint, crouch, and prone.
        /// </summary>
        private void HandleInput()
        {
            // Sprint
            if (Input.GetKeyDown(KeyCode.LeftShift) && CanSprint)
                _isSprinting = true;
            if (Input.GetKeyUp(KeyCode.LeftShift) || !CanSprint)
                _isSprinting = false;
            // Crouch
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                _isCrouching = !_isCrouching;
                _isProne = false;
                onMovementStateChanged?.Invoke(_isCrouching ? "Crouch" : "Normal");
            }
            // Prone
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _isProne = !_isProne;
                _isCrouching = false;
                onMovementStateChanged?.Invoke(_isProne ? "Prone" : "Normal");
            }
        }
        /// <summary>
        /// Handles stamina drain and regeneration.
        /// </summary>
        private void HandleStamina()
        {
            if (_isSprinting)
            {
                _currentStamina -= sprintStaminaDrain * Time.deltaTime;
                if (_currentStamina < 0f) _currentStamina = 0f;
            }
            else
            {
                _currentStamina += staminaRegen * Time.deltaTime;
                if (_currentStamina > maxStamina) _currentStamina = maxStamina;
            }
            onStaminaChanged?.Invoke(_currentStamina / maxStamina);
        }
        private void HandleCameraBobbing()
        {
            if (cameraTransform == null) return;
            float bobSpeed;
            switch (_state)
            {
                case MovementState.Sprint:
                    bobSpeed = bobFrequency * 1.5f;
                    break;
                case MovementState.Crouch:
                    bobSpeed = bobFrequency * 0.7f;
                    break;
                case MovementState.Prone:
                    bobSpeed = bobFrequency * 0.4f;
                    break;
                case MovementState.Aiming:
                    bobSpeed = bobFrequency * 0.5f;
                    break;
                default:
                    bobSpeed = bobFrequency;
                    break;
            }
            if (_input.magnitude > 0.1f)
            {
                _bobTimer += Time.deltaTime * bobSpeed;
                float bobOffset = Mathf.Sin(_bobTimer) * bobAmplitude;
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, _defaultCameraY + bobOffset, cameraTransform.localPosition.z);
            }
            else
            {
                _bobTimer = 0f;
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, _defaultCameraY, cameraTransform.localPosition.z);
            }
        }

        /// <summary>
        /// Callback for the player's movement action (Input System).
        /// Converts the 2D input into a 3D movement vector.
        /// </summary>
        /// <param name="context">Context containing the input data</param>
        public void OnMove(CallbackContext context)
        {
            _input = context.ReadValue<Vector2>();
            _input.z = _input.y;
            _input.y = 0;
        }

        /// <summary>
        /// Callback for the player's look action (Input System).
        /// Retrieves mouse/stick movement data.
        /// </summary>
        /// <param name="context">Context containing the input data</param>
        public void OnLook(CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
    }
}
