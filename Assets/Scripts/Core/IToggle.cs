namespace Core
{
    /// <summary>
    /// Interface defining basic behaviors for objects that can be enabled/disabled.
    /// </summary>
    public interface IToggle
    {
        /// <summary>
        /// Activates the object.
        /// </summary>
        public void Activate();
    
        /// <summary>
        /// Deactivates the object.
        /// </summary>
        public void Deactivate();

        /// <summary>
        /// Toggles the state of the object (active to inactive or vice versa).
        /// </summary>
        public void Switch();
    } 
}
