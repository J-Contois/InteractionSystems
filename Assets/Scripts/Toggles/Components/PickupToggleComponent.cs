using UnityEngine;

using Core;

namespace Toggles.Components
{
    public class PickupToggleComponent : BaseToggleComponent
    {
        [Header("References")]
        [SerializeField] private GameObject pickupObject;
        [SerializeField] private Transform pickupSocket; // Direct reference to the socket
        [SerializeField] private Weapon.WeaponController weaponController; // Reference to WeaponController
        [SerializeField] private bool isWeapon; // Is this pickup a weapon?

        private Rigidbody _rigidbody;
        private Collider _collider;

        private static PickupToggleComponent _currentPickup;
        public static PickupToggleComponent CurrentPickup => _currentPickup;

        private Transform hand;
        private Transform _pickupSocket;

        private void Awake()
        {
            pickupObject ??= gameObject;
            _rigidbody = pickupObject.GetComponent<Rigidbody>();
            _collider = pickupObject.GetComponent<Collider>();
            // No more hierarchy traversal; pickupSocket is assigned directly in inspector
        }

        protected override void DeactivateComponent()
        {
            pickupObject.transform.SetParent(null);
            SetPhysics(enabled: true);
            Debug.Log(_currentPickup.gameObject.name);
            Debug.Log(this.gameObject.name);
            if (_currentPickup == this)
                _currentPickup = null;
            // Notify WeaponController only if this is a weapon
            if (isWeapon && weaponController != null)
            {
                weaponController.OnWeaponDropped(pickupObject);
            }
        }

        protected override void ActivateComponent()
        {
            if (_currentPickup != null && _currentPickup != this)
            {
                _currentPickup.DeactivateComponent();
            }
            _currentPickup = this;
            Debug.Log(_currentPickup.gameObject.name);
            if (pickupObject.transform.parent != null &&
                pickupObject.transform.parent.TryGetComponent<PickupSupport>(out var support))
            {
                Debug.Log("ICI");
                support.ReleaseObject();
            }
            if (pickupSocket == null)
            {
                Debug.LogError("PickupSocket reference is not set in the inspector.");
                return;
            }
            pickupObject.transform.SetParent(pickupSocket, false);
            pickupObject.transform.localPosition = Vector3.zero;
            pickupObject.transform.localRotation = Quaternion.identity;
            SetPhysics(enabled: false);
            // Notify WeaponController only if this is a weapon
            if (isWeapon && weaponController != null)
            {
                weaponController.OnWeaponPickedUp(pickupObject);
            }
        }

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