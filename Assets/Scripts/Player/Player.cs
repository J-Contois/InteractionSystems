using Toggles.Components;
using Toggles.Setters;
using UnityEngine;
using UnityEngineInternal;
using static UnityEngine.InputSystem.InputAction;

namespace Player
{
    /// <summary>
    /// Main controller for the player managing camera movement and rotation.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Motion settings")]
        [Tooltip("Player movement speed in units per second")]
        [SerializeField] private float speed = 5f;
        
        [Tooltip("Camera rotation speed in degrees per second")]
        [SerializeField] private float rotationSpeed = 5f;
    
        [Tooltip("Reference to the player's Rigidbody")]
        [SerializeField] private Rigidbody rigidBody = null;
    
        [Tooltip("Vertical rotation limits (pitch): X = minimum, Y = maximum")]
        [SerializeField] private Vector2 minMaxYaw = new(-90f, 90f);
    
        [Tooltip("Layer mask used to detect interactions with objects.")]
        [SerializeField] private LayerMask interactionMask;
        
        [Header("Transforms References")]
        [Tooltip("Transform player root (horizontal rotation)")]
        [SerializeField] private Transform root = null;
    
        [Tooltip("Head/camera transform (vertical rotation)")]
        [SerializeField] private Transform head = null;
        
        /// <summary>
        /// Normalized displacement input vector.
        /// </summary>
        private Vector3 _input = Vector3.zero;
        
        /// <summary>
        /// Input vector for the gaze (mouse/stick).
        /// </summary>
        private Vector2 _lookInput;
        
        /// <summary>
        /// Current accumulated rotation (X = pitch, Y = yaw).
        /// </summary>
        private Vector2 _currentRotation;
        
        /// <summary>
        /// Initializes the cursor in locked mode at startup.
        /// </summary>
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Automatically called in the editor to initialize references.
        /// </summary>
        private void Reset()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Applies movement to the Rigidbody based on inputs.
        /// Uses FixedUpdate for consistency with the physics engine.
        /// </summary>
        private void FixedUpdate()
        {
            rigidBody.linearVelocity = root.rotation * (speed * _input.normalized);
        }
    
        /// <summary>
        /// Manages camera rotation based on mouse input.
        /// Uses LateUpdate to run after all Updates.
        /// </summary>
        private void LateUpdate()
        {
            _currentRotation.x += -_lookInput.y * rotationSpeed * Time.deltaTime;
            _currentRotation.y += _lookInput.x * rotationSpeed * Time.deltaTime;
            _currentRotation.x = Mathf.Clamp(_currentRotation.x, minMaxYaw.x, minMaxYaw.y);

            root.localRotation = Quaternion.Euler(0f, _currentRotation.y, 0f);
            head.localRotation = Quaternion.Euler(_currentRotation.x, 0f, 0f);
        }
    
        /// <summary>
        /// Callback for the player's movement action (Input System).
        /// Converts the 2D input into a 3D movement vector.
        /// </summary>
        /// <param name="context">Context containing the input data</param>
        public void Player_OnMove(CallbackContext context)
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
        public void Player_OnLook(CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
        
        /// <summary>
        /// Callback for the player's interact action (Input System).
        /// Performs a raycast to detect interactions with objects.
        /// </summary>
        /// <param name="context">Context containing the input data</param>
        public void Player_OnInteract(CallbackContext context)
        {
            if (!context.performed) return;

            Ray ray = new Ray(head.position, head.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f, interactionMask))
            {
                Debug.DrawRay(head.position, transform.TransformDirection(head.forward) * hit.distance, Color.red);

                // If the player is holding an object and looking at a support → place
                if (hit.collider.transform.parent != null &&
                    hit.collider.transform.parent.TryGetComponent(out PickupSupport support))
                {
                    if (PickupToggleComponent.CurrentPickup != null)
                        PickupToggleComponent.CurrentPickup.TryPlaceOn(support);
                }

                // If the player is holding an object and is not looking at a support → release freely
                if (hit.collider.TryGetComponent(out PickupToggleComponent pickup))
                {
                    Debug.Log("Passer PickupToggleComponent : " + PickupToggleComponent.CurrentPickup);
                    if (PickupToggleComponent.CurrentPickup != null && PickupToggleComponent.CurrentPickup != pickup)
                        PickupToggleComponent.CurrentPickup.Deactivate();

                    pickup.Activate();
                    return;
                }
                
                if (hit.collider.TryGetComponent(out PuzzleButton button))
                {
                    button.PressButton();
                    return;
                }

                // Otherwise, classic interaction via InteractionToggleSetter
                if (hit.collider.TryGetComponent(out InteractionToggleSetter interactionToggleSetter))
                {
                    interactionToggleSetter.Interact();
                }
            }
            else
            {
                // No raycast hit - if the player is holding an object, drop it freely
                if (PickupToggleComponent.CurrentPickup != null)
                    PickupToggleComponent.CurrentPickup.Deactivate();
            }
        }
    }
}
