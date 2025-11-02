using UnityEngine;

using Core;

namespace Toggles.Components
{
    /// <summary>
    /// Component that controls an Animator by triggering animations.
    /// Can be used for doors, bridges, traps, or any animated object.
    /// Provides methods to play activation/deactivation animations
    /// and keeps track of the open/closed state.
    /// </summary>
    public class AnimatorToggleComponent : BaseToggleComponent
    {
        [Header("Animation Settings")]
        [Tooltip("The Animator to control")]
        [SerializeField] private Animator animator = null;
        
        [Tooltip("If true, starts the animation in play mode")]
        [SerializeField] private bool startInPlayMode = false;
        
        [Tooltip("Animation trigger name for activation")]
        [SerializeField] private string activateTriggerName = "Activate";
        
        [Tooltip("Animation trigger name for deactivation")]
        [SerializeField] private string deactivateTriggerName = "Deactivate";
        
        [Tooltip("Indicates whether the object is currently open")]
        [SerializeField] private bool isOpen = false;

        /// <summary>
        /// Automatically called in the editor to reset references.
        /// Ensures an Animator is assigned.
        /// </summary>
        private void Reset()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }
        
        /// <summary>
        /// Called when the component awakes.
        /// Optionally disables animator if not starting in play mode.
        /// </summary>
        private void Awake()
        {
            if (!startInPlayMode && animator != null)
                animator.Update(0);
        }
        
        /// <summary>
        /// Activates this component by playing the activation animation.
        /// </summary>
        protected override void ActivateComponent()
        {
            PlayActivateAnimation();
        }
        
        /// <summary>
        /// Deactivates this component by playing the deactivation animation.
        /// </summary>
        protected override void DeactivateComponent()
        {
            PlayDeactivateAnimation();
        }
        
        /// <summary>
        /// Public method to play the activation animation, regardless of state
        /// </summary>
        public void PlayActivateAnimation()
        {
            if (animator == null || isOpen) return;
            
            animator.ResetTrigger(activateTriggerName);
            
            animator.SetTrigger(activateTriggerName);
            isOpen = true;
        }

        /// <summary>
        /// Public method to play the deactivation animation, regardless of state
        /// </summary>
        public void PlayDeactivateAnimation()
        {
            if (animator == null || !isOpen) return;
            
            animator.ResetTrigger(deactivateTriggerName);
            
            animator.SetTrigger(deactivateTriggerName);
            isOpen = false;
        }
    }
}
