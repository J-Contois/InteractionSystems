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
        
        [Tooltip("If true, starts the animation in play mode")]
        [SerializeField] private bool startInPlayMode = false;
        
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
        
        private void Awake()
        {
            if (!startInPlayMode && animator != null)
            {
                animator.enabled = false;
                
                animator.ResetTrigger(activateTriggerName);
                animator.ResetTrigger(deactivateTriggerName);
            }
        }

        protected override void ActivateComponent()
        {
            if (animator != null)
            {
                Debug.Log($"{gameObject.name} - Triggering animation: {activateTriggerName}");
                animator.enabled = true;
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
                animator.enabled = true;
                animator.SetTrigger(deactivateTriggerName);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - No Animator assigned!");
            }
        }
    }
}
