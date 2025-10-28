using UnityEngine;
using Core;

namespace Toggles.Components
{
    public class PickupToggleComponent : BaseToggleComponent
    {
        [Header("Références")]
        [SerializeField] private GameObject pickupObject;
        [SerializeField] private Transform hand;
        [SerializeField] private bool returnToOriginalPosition = true;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Transform _originalParent;

        private Rigidbody _rigidbody;
        private Collider _collider;

        private static GameObject _currentHeldObject;
        private static PickupToggleComponent _currentPickup;

        private void Awake()
        {
            if (pickupObject == null) pickupObject = this.gameObject;

            _originalPosition = pickupObject.transform.position;
            _originalRotation = pickupObject.transform.rotation;
            _originalParent = pickupObject.transform.parent;

            _rigidbody = pickupObject.GetComponent<Rigidbody>();
            _collider = pickupObject.GetComponent<Collider>();
        }

        protected override void ActivateComponent()
        {
            if (_currentHeldObject != null && _currentHeldObject != pickupObject)
            {
                _currentPickup.DeactivateComponent();
            }

            _currentHeldObject = pickupObject;
            _currentPickup = this;

            pickupObject.transform.SetParent(hand);
            pickupObject.transform.localPosition = Vector3.zero;
            pickupObject.transform.localRotation = Quaternion.identity;

            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
            }

            if (_collider != null)
            {
                _collider.enabled = false;
            }
        }

        protected override void DeactivateComponent()
        {
            if (returnToOriginalPosition)
                ReturnToOrigin();
            else
                DropObject();
        }

        private void ReturnToOrigin()
        {
            pickupObject.transform.SetParent(_originalParent);
            pickupObject.transform.position = _originalPosition;
            pickupObject.transform.rotation = _originalRotation;

            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
            }

            if (_collider != null)
            {
                _collider.enabled = true;
            }

            if (_currentHeldObject == pickupObject)
            {
                _currentHeldObject = null;
                _currentPickup = null;
            }
        }

        private void DropObject()
        {
            pickupObject.transform.SetParent(null);

            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
            }

            if (_collider != null)
            {
                _collider.enabled = true;
            }

            if (_currentHeldObject == pickupObject)
            {
                _currentHeldObject = null;
                _currentPickup = null;
            }
        }
    }
}
