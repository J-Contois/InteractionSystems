using UnityEngine;
using Core;

namespace Toggles.Components
{
    /// <summary>
    /// Component that activates only when all specified conditions are met.
    /// Checks if all PickupSupports have objects placed on them.
    /// </summary>
    public class MultiConditionToggleComponent : BaseToggleComponent
    {
        [Header("Conditions")]
        [Tooltip("List of PickupSupports that must all have objects placed")]
        [SerializeField] private PickupSupport[] requiredSupports;

        [Header("Target to activate")]
        [Tooltip("The toggle component to activate when conditions are met")]
        [SerializeField] private BaseToggleComponent targetComponent;

        /// <summary>
        /// Checks if all conditions are met.
        /// </summary>
        /// <returns>True if all supports have objects placed</returns>
        private bool AreConditionsMet()
        {
            if (requiredSupports == null || requiredSupports.Length == 0)
            {
                Debug.LogWarning("No required supports assigned!");
                return false;
            }

            foreach (var support in requiredSupports)
            {
                if (support == null)
                {
                    Debug.LogWarning("Null support in required supports array!");
                    continue;
                }

                if (!support.HasCorrectObject)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void ActivateComponent()
        {
            if (AreConditionsMet())
            {
                Debug.Log("All conditions met! Activating target...");
                if (targetComponent != null)
                {
                    targetComponent.Activate();
                }
                else
                {
                    Debug.LogWarning("No target component assigned!");
                }
            }
            else
            {
                Debug.Log("Conditions not met. All supports must have their objects.");
            }
        }

        protected override void DeactivateComponent()
        {
            Debug.Log("Deactivating target...");
            if (targetComponent != null)
            {
                targetComponent.Deactivate();
            }
        }
        
        public void ValidatePuzzle()
        {
            Activate();
        }
    }
}
