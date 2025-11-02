using Core;

namespace Toggles.Setters
{
    /// <summary>
    /// Setter for interaction toggles. Calls Switch() on BaseToggleSetter.
    /// </summary>
    public class InteractionToggleSetter : BaseToggleSetter
    {
        public void Interact()
        {
            Switch();
        }
    }
}
