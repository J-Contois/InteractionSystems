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
    
        /// <inheritdoc/>
        /// <remarks>
        /// This implementation delegates the call to BaseToggleComponent.Activate().
        /// </remarks>
        public void Activate()
        {
            toggleComponent.Activate();
        }
    
        /// <inheritdoc/>
        /// <remarks>
        /// This implementation delegates the call to BaseToggleComponent.Deactivate().
        /// </remarks>
        public void Deactivate()
        {
            toggleComponent.Deactivate();
        }
    
        /// <inheritdoc/>
        /// <remarks>
        /// This implementation delegates the call to BaseToggleComponent.Switch().
        /// </remarks>
        public void Switch()
        {
            toggleComponent.Switch();
        }
    }
}
