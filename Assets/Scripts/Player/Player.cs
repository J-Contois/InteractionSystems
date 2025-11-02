using Core;
using Toggles.Components;
using Toggles.Setters;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Player
{
    /// <summary>
    /// Main controller for the player. Handles interaction logic. Movement/rotation is in PlayerMovement.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Motion settings")]
        [Tooltip("Layer mask used to detect interactions with objects.")]
        [SerializeField] private LayerMask interactionMask;

        /// <summary>
        /// Initializes the cursor in locked mode at startup.
        /// </summary>
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Callback for the player's interact action (Input System).
        /// Performs a raycast to detect interactions with objects.
        /// </summary>
        /// <param name="context">Context containing the input data</param>
        public void Player_OnInteract(CallbackContext context)
        {
            if (!context.performed) return;

            // Use Camera.main.transform for ray origin/direction if head is removed
            Transform rayOrigin = Camera.main ? Camera.main.transform : transform;
            Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f, interactionMask))
            {
                Debug.Log("Ray");
                Debug.DrawRay(rayOrigin.position, rayOrigin.forward * hit.distance, Color.red);

                // If the player is holding an object and looking at a support → place
                if (hit.collider.transform.parent != null &&
                    hit.collider.transform.parent.TryGetComponent(out PickupSupport support))
                {
                    if (PickupToggleComponent.CurrentPickup != null)
                    {
                        PickupToggleComponent.CurrentPickup.TryPlaceOn(support);
                    }
                }

                // If the player is holding an object and is not looking at a support → release freely
                if (hit.collider.TryGetComponent(out PickupToggleComponent pickup))
                {
                    Debug.Log("Passer PickupToggleComponent : " + PickupToggleComponent.CurrentPickup);
                    if (PickupToggleComponent.CurrentPickup != null && PickupToggleComponent.CurrentPickup != pickup)
                    {
                        Debug.Log("TryPass");
                        PickupToggleComponent.CurrentPickup.Deactivate();
                    }

                    pickup.Activate();
                    return;
                }

                // Otherwise, classic interaction via InteractionToggleSetter
                if (hit.collider.TryGetComponent(out InteractionToggleSetter interactionToggleSetter))
                {
                    interactionToggleSetter.Interact();
                }

                // Interact with lootable dead enemy (BaseToggleComponent)
                var toggle = hit.collider.GetComponentInParent<BaseToggleComponent>();
                if (toggle != null)
                {
                    toggle.Activate();
                    return;
                }
            }
            else
            {
                // No raycast hit - if the player is holding an object, drop it freely
                if (PickupToggleComponent.CurrentPickup != null)
                {
                    Debug.Log("Drop object freely");
                    PickupToggleComponent.CurrentPickup.Deactivate();
                }
            }

        }
    }
}
