using UnityEngine;

using Toggles.Components;
public class PuzzleButton : MonoBehaviour
{
    [SerializeField] private MultiConditionToggleComponent puzzleManager;

    // Appelé par le Player_OnInteract lorsque le joueur clique sur le bouton
    public void PressButton()
    {
        if (puzzleManager != null)
        {
            puzzleManager.ValidatePuzzle();
        }
    }
}
