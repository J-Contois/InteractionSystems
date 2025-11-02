using UnityEngine;
using Core;

namespace Toggles.Components
{
    /// <summary>
    /// Component that activates a target AnimatorToggleComponent only when
    /// all specified puzzle conditions are met. 
    /// Checks if all linked PickupSupports have the correct objects placed.
    /// </summary>
    public class MultiConditionToggleComponent : BaseToggleComponent
    {
        [Header("Puzzle Supports Validation")]
        [Tooltip("List of links between media and objects needed to solve the puzzle")]
        [SerializeField] private PuzzleSupportLinker[] puzzleLinks = null;

        [Header("Target to activate")]
        [Tooltip("The toggle component to activate when conditions are met")]
        [SerializeField] private AnimatorToggleComponent targetComponent = null;

        /// <summary>
        /// Checks if all conditions are met.
        /// </summary>
        /// <returns>True if all supports have the correct object placed</returns>
        private bool AreConditionsMet()
        {
            if (puzzleLinks == null || puzzleLinks.Length == 0)
            {
                Debug.LogWarning("No PuzzleSupportLinker assigned in the puzzle manager!");
                return false;
            }

            foreach (var link in puzzleLinks)
            {
                if (link == null || !link.IsValid())
                {
                    Debug.Log($"Condition failed for {link?.name}");
                    return false;
                }
            }
            
            Debug.Log("All conditions are met!");
            return true;
        }

        /// <summary>
        /// Empty override to prevent default activation behavior.
        /// </summary>
        protected override void ActivateComponent() {}

        /// <summary>
        /// Empty override to prevent default deactivation behavior.
        /// </summary>
        protected override void DeactivateComponent() {}

        /// <summary>
        /// Checks puzzle conditions and opens/closes the target animator accordingly.
        /// </summary>
        public void ValidatePuzzle()
        {
            if (targetComponent == null) return;
            
            if (AreConditionsMet())
            {
                targetComponent.PlayActivateAnimation();
            }
            else
            {
                targetComponent.PlayDeactivateAnimation();
            }
        }
    }
}
