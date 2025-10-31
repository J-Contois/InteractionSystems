using UnityEngine;
using Core;

namespace Toggles.Components
{
    /// <summary>
    /// Component that controls an Animator by triggering animations.
    /// Used for doors, bridges, traps, or any animated object.
    /// </summary>
    public class AnimatorToggleComponent : BaseToggleComponent
    {
        [Header("Animation Settings")]
        [Tooltip("The Animator to control")]
        [SerializeField] private Animator animator;
        
        [Tooltip("Animation trigger name for activation")]
        [SerializeField] private string activateTriggerName = "Activate";
        
        [Tooltip("Animation trigger name for deactivation")]
        [SerializeField] private string deactivateTriggerName = "Deactivate";

        private void Reset()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        protected override void ActivateComponent()
        {
            if (animator != null)
            {
                Debug.Log($"{gameObject.name} - Triggering animation: {activateTriggerName}");
                animator.SetTrigger(activateTriggerName);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - No Animator assigned!");
            }
        }

        protected override void DeactivateComponent()
        {
            if (animator != null)
            {
                Debug.Log($"{gameObject.name} - Triggering animation: {deactivateTriggerName}");
                animator.SetTrigger(deactivateTriggerName);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - No Animator assigned!");
            }
        }
    }
}
