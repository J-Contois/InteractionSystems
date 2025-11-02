using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// Links a PuzzleSupport to the expected object for a puzzle slot.
/// Used to validate whether the correct object has been placed on a support.
/// </summary>
public class PuzzleSupportLinker : MonoBehaviour
{
    [Tooltip("The support associated with this slot puzzle")]
    [SerializeField] private PickupSupport support = null;

    [Tooltip("The correct object expected on this medium to solve the puzzle")]
    [SerializeField] private GameObject expectedObject = null;

    /// <summary>
    /// Checks if the object currently placed on the linked support
    /// is the expected one for this puzzle slot.
    /// </summary>
    /// <returns>
    /// True if the support has an object and it matches the expected object;
    /// otherwise, false
    /// </returns>
    public bool IsValid()
    {
        if (support == null)
            return false;

        var current = support.HasObject ? support.GetPlacedObject() : null;
        return current != null && current == expectedObject;
    }
}