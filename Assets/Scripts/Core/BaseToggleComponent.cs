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

        /// <summary>
        /// Activates the component and calls ActivateComponent().
        /// </summary>
        public void Activate()
        {
            _state = true;
            ActivateComponent();
        }

        /// <summary>
        /// Deactivates the component and calls DeactivateComponent().
        /// </summary>
        public void Deactivate()
        {
            _state = false;
            DeactivateComponent();
        }

        /// <summary>
        /// Abstract method called during activation. Must be implemented in derived classes.
        /// </summary>
        protected abstract void ActivateComponent();

        /// <summary>
        /// Abstract method called during deactivation. Must be implemented in derived classes.
        /// </summary>
        protected abstract void DeactivateComponent();

        /// <summary>
        /// Switches the state of the component (active/inactive).
        /// </summary>
        public void Switch()
        {
            if (_state)
                Deactivate();
            else
                Activate();
        }
    }
}
