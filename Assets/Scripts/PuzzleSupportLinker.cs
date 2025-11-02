using UnityEngine;

[DisallowMultipleComponent]
public class PuzzleSupportLinker : MonoBehaviour
{
    [Tooltip("The support associated with this slot puzzle")]
    [SerializeField] private PickupSupport support = null;

    [Tooltip("The correct object expected on this medium to solve the puzzle")]
    [SerializeField] private GameObject expectedObject = null;

    public bool IsValid()
    {
        if (support == null)
            return false;

        var current = support.HasObject ? support.GetPlacedObject() : null;
        return current != null && current == expectedObject;
    }
}