using UnityEngine;

using Toggles.Components;
public class PuzzleButton : MonoBehaviour
{
    [SerializeField] private MultiConditionToggleComponent puzzleManager = null;
    
    [SerializeField] private AnimatorToggleComponent animatorButton = null;
    
    /// <summary>
    /// Called when the player presses the button.
    /// Plays the button animation, then validates the puzzle.
    /// </summary>
    public void PressButton()
    {
        // Play button press animation regardless of puzzle state
        animatorButton.PlayActivateAnimation();
        animatorButton.PlayDeactivateAnimation();
        
        // Validate the puzzle (open/close door accordingly)
        puzzleManager?.ValidatePuzzle();
    }
}
