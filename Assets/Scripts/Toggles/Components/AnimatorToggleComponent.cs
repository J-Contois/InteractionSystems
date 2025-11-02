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
        [SerializeField] private Animator animator = null;
        
        [Tooltip("If true, starts the animation in play mode")]
        [SerializeField] private bool startInPlayMode = false;
        
        [Tooltip("Animation trigger name for activation")]
        [SerializeField] private string activateTriggerName = "Activate";
        
        [Tooltip("Animation trigger name for deactivation")]
        [SerializeField] private string deactivateTriggerName = "Deactivate";
        
        [SerializeField] private bool isOpen = false;

        private void Reset()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
        }
        
        private void Awake()
        {
            if (!startInPlayMode && animator != null)
                animator.Update(0);
        }
        
        protected override void ActivateComponent()
        {
            PlayActivateAnimation();
        }
        
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
