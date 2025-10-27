using UnityEngine;

namespace Core
{
    /// <summary>
    /// Abstract base class for components that can be enabled/disabled.
    /// Manages the internal state and delegates specific implementation to derived classes.
    /// </summary>
    public abstract class BaseToggleComponent : MonoBehaviour, IToggle
    {
        private bool _state = false;
    
        /// <inheritdoc/> 
        public void Activate()
        {
            _state = true;
            ActivateComponent();
        }
    
        /// <inheritdoc/> 
        public void Deactivate()
        {
            _state = false;
            DeactivateComponent();
        }

        /// <summary>
        /// Abstract method called during activation. 
        /// Must be implemented in derived classes to define specific behavior.
        /// </summary>
        protected abstract void ActivateComponent();
    
        /// <summary>
        /// Abstract method called during deactivation.
        /// Must be implemented in derived classes to define specific behavior.
        /// </summary>
        protected abstract void DeactivateComponent();
    
        /// <inheritdoc/> 
        public void Switch()
        {
            if (_state)
                Deactivate();
            else
                Activate();
        }
    }
}
