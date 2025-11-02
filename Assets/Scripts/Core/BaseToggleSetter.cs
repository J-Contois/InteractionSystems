using UnityEngine;

namespace Core
{
    /// <summary>
    /// Intermediate class that passes toggle commands to a BaseToggleComponent component.
    /// Allows control logic to be separated from implementation logic.
    /// </summary>
    public class BaseToggleSetter : MonoBehaviour, IToggle
    {
        [Tooltip("Toggle component that will be controlled by this setter")]
        [SerializeField] private BaseToggleComponent toggleComponent = null;

        /// <summary>
        /// Activates the controlled toggle component.
        /// </summary>
        public void Activate()
        {
            toggleComponent.Activate();
        }

        /// <summary>
        /// Deactivates the controlled toggle component.
        /// </summary>
        public void Deactivate()
        {
            toggleComponent.Deactivate();
        }

        /// <summary>
        /// Switches the controlled toggle component.
        /// </summary>
        public void Switch()
        {
            toggleComponent.Switch();
        }
    }
}
