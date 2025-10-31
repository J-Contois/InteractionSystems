using UnityEngine;

using Core;

namespace Toggles.Components
{
    public class PickupToggleComponent : BaseToggleComponent
    {
        [Header("References")]
        [SerializeField] private GameObject pickupObject;
        [SerializeField] private Transform hand;

        private Rigidbody _rigidbody;
        private Collider _collider;

        private static PickupToggleComponent _currentPickup;
        public static PickupToggleComponent CurrentPickup => _currentPickup;

        private void Awake()
        {
            pickupObject ??= gameObject;

            _rigidbody = pickupObject.GetComponent<Rigidbody>();
            _collider = pickupObject.GetComponent<Collider>();
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

            pickupObject.transform.SetParent(hand, false);
            pickupObject.transform.localPosition = Vector3.zero;
            pickupObject.transform.localRotation = Quaternion.identity;

            SetPhysics(enabled: false);
        }

        protected override void DeactivateComponent()
        {
            pickupObject.transform.SetParent(null);
            SetPhysics(enabled: true);
            Debug.Log(_currentPickup.gameObject.name);
            Debug.Log(this.gameObject.name);
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
