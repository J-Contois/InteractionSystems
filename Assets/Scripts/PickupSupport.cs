using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// Represents a support point where the player can place pickup objects.
/// Can validate objects either by a specific instance or by layer.
/// Manages placement position, rotation, and physics of the placed object.
/// </summary>
public class PickupSupport : MonoBehaviour
{
    /// <summary>
    /// Determines how the support validates objects placed on it.
    /// </summary>
    public enum ValidationMode
    {
        SpecificObject,  // Specific GameObject instance
        ObjectWithLayer    // Any GameObject with a specific layers tag
    }

    [Header("Pickup Validation")]
    [Tooltip("How to validate if an object can be placed")]
    [SerializeField] private ValidationMode validationMode = ValidationMode.ObjectWithLayer;

    [Header("Specific Object Mode")]
    [Tooltip("The specific GameObject instance allowed (only used if mode is SpecificObject)")]
    [SerializeField] private GameObject allowedPickup = null;

    [Header("Layer Mode")]
    [Tooltip("Layer that pickup objects must have (only used if mode is ObjectWithLayer)")]
    [SerializeField] private string requiredLayerName = null;

    [Header("Placement")]
    [Tooltip("Transform to place the object")]
    [SerializeField] private Transform placementPoint = null;

    private GameObject _currentObject;
    
    /// <summary>
    /// Gets the object currently placed on this support.
    /// </summary>
    /// <returns>The currently placed GameObject, or null if empty</returns>
    public GameObject GetPlacedObject() => _currentObject;
    
    /// <summary>
    /// True if this support currently has an object placed on it.
    /// </summary>
    public bool HasObject => _currentObject != null;
    
    /// <summary>
    /// True if the placed object is the correct one for this support
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
                ValidationMode.ObjectWithLayer => _currentObject.layer == LayerMask.NameToLayer(requiredLayerName),
                _ => false
            };
        }
    }

    /// <summary>
    /// Determines if the given object can be placed on this support.
    /// </summary>
    /// <param name="obj">The object to check</param>
    /// <returns>True if the object can be placed; otherwise, false</returns>
    public bool CanPlace(GameObject obj)
    {
        if (obj == null || _currentObject != null)
            return false;

        return validationMode switch
        {
            ValidationMode.SpecificObject => obj == allowedPickup,
            ValidationMode.ObjectWithLayer => obj.layer == LayerMask.NameToLayer(requiredLayerName),
            _ => false
        };

    }
    
    /// <summary>
    /// Places the given object on this support.
    /// </summary>
    /// <param name="obj">The object to place</param>
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
    /// Release the object currently on this support
    /// </summary>
    public void ReleaseObject()
    {
        _currentObject = null;
    }
}
