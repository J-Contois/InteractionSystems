using Core;

namespace Toggles.Setters
{
    /// <summary>
    /// Setter for interaction toggles. Calls Switch() on BaseToggleSetter.
    /// </summary>
    public class InteractionToggleSetter : BaseToggleSetter
    {
        /// <summary>
        /// Calls Switch() to interact with the toggle.
        /// </summary>
        public void Interact()
        {
            Switch();
        }
    }
}
