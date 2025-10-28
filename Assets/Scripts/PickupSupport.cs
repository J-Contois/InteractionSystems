using UnityEngine;

[DisallowMultipleComponent]
public class PickupSupport : MonoBehaviour
{
    [Header("Pickup allowed")]
    [SerializeField] private GameObject allowedPickup;

    [SerializeField] private Transform placementPoint;

    private GameObject _currentObject;

    public bool CanPlace(GameObject obj) => obj == allowedPickup && _currentObject == null;

    public void PlaceObject(GameObject obj)
    {
        if (!CanPlace(obj)) return;

        _currentObject = obj;
        Transform t = placementPoint != null ? placementPoint : transform;

        obj.transform.SetParent(t, worldPositionStays: false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (obj.TryGetComponent<Collider>(out var collider))
            collider.enabled = false;
    }

    public void ReleaseObject()
    {
        _currentObject = null;
    }
}
