using UnityEngine;

[DisallowMultipleComponent]
public class PickupSupport : MonoBehaviour
{
    public enum ValidationMode
    {
        SpecificObject,  // Specific GameObject instance
        ObjectWithTag    // Any GameObject with a specific layers tag
    }

    [Header("Pickup Validation")]
    [Tooltip("How to validate if an object can be placed")]
    [SerializeField] private ValidationMode validationMode = ValidationMode.ObjectWithTag;

    [Header("Specific Object Mode")]
    [Tooltip("The specific GameObject instance allowed (only used if mode is SpecificObject)")]
    [SerializeField] private GameObject allowedPickup;

    [Header("Tag Mode")]
    [Tooltip("Tag that pickup objects must have (only used if mode is ObjectWithTag)")]
    [SerializeField] private string requiredTag = "Untagged";

    [Header("Placement")]
    [SerializeField] private Transform placementPoint;

    private GameObject _currentObject;
    
    /// <summary>
    /// Returns true if this support currently has an object placed on it.
    /// </summary>
    public bool HasObject => _currentObject != null;
    
    /// <summary>
    /// Returns true if this support has the CORRECT object placed on it.
    /// </summary>
    public bool HasCorrectObject
    {
        get
        {
            if (_currentObject == null)
                return false;

            return validationMode switch
            {
                ValidationMode.SpecificObject => _currentObject == allowedPickup,
                ValidationMode.ObjectWithTag => _currentObject.CompareTag(requiredTag),
                _ => false
            };
        }
    }

    /// <summary>
    /// Checks if an object can be placed on this support.
    /// </summary>
    public bool CanPlace(GameObject obj)
    {
        if (obj == null || _currentObject != null)
            return false;

        return validationMode switch
        {
            ValidationMode.SpecificObject => obj == allowedPickup,
            ValidationMode.ObjectWithTag => obj.CompareTag(requiredTag),
            _ => false
        };

    }
    
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

    public void ReleaseObject()
    {
        _currentObject = null;
    }
}
