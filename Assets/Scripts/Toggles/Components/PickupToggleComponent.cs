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

        public static PickupToggleComponent _currentPickup;

        private void Awake()
        {
            pickupObject ??= gameObject;

            _rigidbody = pickupObject.GetComponent<Rigidbody>();
            _collider = pickupObject.GetComponent<Collider>();
        }

        protected override void ActivateComponent()
        {
            _currentPickup?.DeactivateComponent();

            _currentPickup = this;

            SetTransformLocal(hand, Vector3.zero, Quaternion.identity, Vector3.one);

            SetPhysics(enabled: false);
        }

        protected override void DeactivateComponent()
        {
            pickupObject.transform.SetParent(null);
            SetPhysics(enabled: true);

            if (_currentPickup == this)
                _currentPickup = null;
        }

        private void SetTransformLocal(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            pickupObject.transform.SetParent(parent, worldPositionStays: false);
            pickupObject.transform.localPosition = localPosition;
            pickupObject.transform.localRotation = localRotation;
            pickupObject.transform.localScale = localScale;
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
