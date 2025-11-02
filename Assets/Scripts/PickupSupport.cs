using UnityEngine;

/// <summary>
/// Handles logic for placing and releasing pickup objects on a support point.
/// </summary>
[DisallowMultipleComponent]
public class PickupSupport : MonoBehaviour
{
    [Header("Pickup allowed")]
    [Tooltip("The only object allowed to be placed here.")]
    [SerializeField] private GameObject allowedPickup;
    [Tooltip("Transform where the object will be placed.")]
    [SerializeField] private Transform placementPoint;

    private GameObject _currentObject;

    /// <summary>
    /// Returns true if the given object can be placed here.
    /// </summary>
    public bool CanPlace(GameObject obj) => obj == allowedPickup && _currentObject == null;

    /// <summary>
    /// Places the object at the placement point and disables its physics.
    /// </summary>
    public void PlaceObject(GameObject obj)
    {
        if (!CanPlace(obj)) return;

        _currentObject = obj;
        Transform t = placementPoint != null ? placementPoint : transform;

        obj.transform.SetParent(t, worldPositionStays: false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (obj.TryGetComponent<Collider>(out var collider))
            collider.enabled = true;
    }

    /// <summary>
    /// Releases the currently placed object.
    /// </summary>
    public void ReleaseObject()
    {
        _currentObject = null;
    }
}
