using UnityEngine;

using Core;

namespace Toggles.Components
{
    public class PickupToggleComponent : BaseToggleComponent
    {
        [Header("References")]
        [SerializeField] private GameObject pickupObject = null;
        [SerializeField] private Transform hand = null;

        private Rigidbody _rigidbody;
        private Collider _collider;

        private static PickupToggleComponent _currentPickup;
        public static PickupToggleComponent CurrentPickup => _currentPickup;

        private void Reset()
        {
            if (pickupObject == null)
                pickupObject = gameObject;
        }
        
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

        protected override void ActivateComponent()
        {
            if (_currentPickup != null && _currentPickup != this)
                _currentPickup.DeactivateComponent();

            _currentPickup = this;

            if (pickupObject.transform.parent != null &&
                pickupObject.transform.parent.TryGetComponent<PickupSupport>(out var support))
            {
                support.ReleaseObject();
            }

            pickupObject.transform.SetParent(hand, false);
            pickupObject.transform.localPosition = Vector3.zero;
            pickupObject.transform.localRotation = Quaternion.identity;

            SetPhysics(enabled: false);
        }

        protected override void DeactivateComponent()
        {
            pickupObject.transform.SetParent(null);
            SetPhysics(enabled: true);

            if (_currentPickup == this)
                _currentPickup = null;
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

            return true;
        }
    }
}
