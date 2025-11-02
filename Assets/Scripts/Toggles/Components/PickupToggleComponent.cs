using UnityEngine;

using Core;

namespace Toggles.Components
{
    /// <summary>
    /// Component that allows a player to pick up an object and hold it in hand.
    /// Handles parenting, physics, and interaction with PickupSupports.
    /// Handles pickup logic for objects, including weapons. Inherits from BaseToggleComponent.
    /// </summary>
    public class PickupToggleComponent : BaseToggleComponent
    {
        [Header("References")]
        [Tooltip("The GameObject to pick up")]
        [SerializeField] private GameObject pickupObject = null;
        
        [Tooltip("The Transform where the picked object will be held")]
        [SerializeField] private Transform hand = null;

        [Tooltip("Reference to WeaponController if this is a weapon.")]
        [SerializeField] private Weapon.WeaponController weaponController;
        
        [Tooltip("Is this pickup a weapon?")]
        [SerializeField] private bool isWeapon;

        private Rigidbody _rigidbody;
        private Collider _collider;

        private static PickupToggleComponent _currentPickup;
        
        /// <summary>
        /// The currently held PickupToggleComponent instance.
        /// </summary>
        public static PickupToggleComponent CurrentPickup => _currentPickup;

        /// <summary>
        /// Automatically assign the pickupObject to self if not set in inspector.
        /// </summary>
        private void Reset()
        {
            if (pickupObject == null)
                pickupObject = gameObject;
        }
        
        /// <summary>
        /// Initializes references and warns if mandatory fields are missing.
        /// </summary>
        private void Awake()
        {
            if (pickupObject == null)
            {
                pickupObject = gameObject;
                Debug.LogWarning($"{gameObject.name}: pickupObject was null at runtime, assigned to self.");
            }

            if (hand == null)
                Debug.LogWarning($"{gameObject.name}: hand reference is not set!");
            
            _rigidbody = pickupObject.GetComponent<Rigidbody>();
            _collider = pickupObject.GetComponent<Collider>();
        }

        /// <summary>
        /// Activates the pickup, attaching the object to the player's hand
        /// and disabling physics.
        /// </summary>
        protected override void ActivateComponent()
        {
            if (_currentPickup != null && _currentPickup != this)
                _currentPickup.DeactivateComponent();

            _currentPickup = this;

            if (pickupObject.transform.parent != null &&
                pickupObject.transform.parent.TryGetComponent<PickupSupport>(out var support))
            {
                Debug.Log("ICI");
                support.ReleaseObject();
            }

            pickupObject.transform.SetParent(hand, false);
            pickupObject.transform.localPosition = Vector3.zero;
            pickupObject.transform.localRotation = Quaternion.identity;

            SetPhysics(enabled: false);
            
            // Notify WeaponController only if this is a weapon
            if (isWeapon && weaponController != null)
            {
                weaponController.OnWeaponPickedUp(pickupObject);
            }
        }

        /// <summary>
        /// Handles deactivation logic: detaches object,
        /// enables physics, notifies WeaponController if weapon.
        /// </summary>
        protected override void DeactivateComponent()
        {
            pickupObject.transform.SetParent(null);
            SetPhysics(enabled: true);

            if (_currentPickup == this)
                _currentPickup = null;
            
            // Notify WeaponController only if this is a weapon
            if (isWeapon && weaponController != null)
            {
                weaponController.OnWeaponDropped(pickupObject);
            }
        }

        /// <summary>
        /// Enables or disables physics on the pickup object.
        /// </summary>
        /// <param name="enabled">If true, physics is enabled; otherwise disabled.</param>
        private void SetPhysics(bool enabled)
        {
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = !enabled;
                _rigidbody.useGravity = enabled;
            }

            if (_collider != null)
                _collider.enabled = enabled;
        }

        /// <summary>
        /// Attempts to place the pickup object on a PickupSupport. Handles unequip logic for weapons.
        /// </summary>
        /// <param name="support">The PickupSupport to place the object on.</param>
        /// <returns>True if the object was successfully placed; false otherwise.</returns>
        public bool TryPlaceOn(PickupSupport support)
        {
            if (support == null || !support.CanPlace(pickupObject))
                return false;

            support.PlaceObject(pickupObject);

            _currentPickup = null;

            // If this is a weapon, reflect unequip logic
            if (isWeapon && weaponController != null)
            {
                weaponController.OnWeaponDropped(pickupObject);
            }

            return true;
        }
    }
}
