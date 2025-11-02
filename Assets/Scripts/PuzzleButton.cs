using UnityEngine;

using Toggles.Components;
public class PuzzleButton : MonoBehaviour
{
    [SerializeField] private MultiConditionToggleComponent puzzleManager = null;
    
    [SerializeField] private AnimatorToggleComponent animatorButton = null;
    
    public void PressButton()
    {
        animatorButton.PlayActivateAnimation();
        animatorButton.PlayDeactivateAnimation();
        
        puzzleManager?.ValidatePuzzle();
    }
}
