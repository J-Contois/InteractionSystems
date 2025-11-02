using Core;

namespace Toggles.Setters
{
    public class InteractionToggleSetter : BaseToggleSetter
    {
        /// <summary>
        /// Simple toggle setter that switches state when interacted with
        /// </summary>
        public void Interact()
        {
            Switch();
        }
    }
}
