using Core;
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles highlighting and looting logic for dead enemies. Inherits from BaseToggleComponent.
/// </summary>
public class EnemyLootToggle : BaseToggleComponent
{
    [Header("Highlight")]
    [Tooltip("Effect to highlight the enemy when lootable.")]
    [SerializeField] public Behaviour highlightEffect;
    private bool canLoot = false;
    private bool isLooted = false;
    /// <summary>
    /// Enables looting and highlights the enemy.
    /// </summary>
    public void EnableLoot()
    {
        canLoot = true;
        if (highlightEffect != null)
            highlightEffect.enabled = true;
    }

    /// <summary>
    /// Activates looting, disables highlight, and removes enemy after delay.
    /// </summary>
    public new void Activate()
    {
        if (!canLoot || isLooted) return;
        isLooted = true;
        canLoot = false;
        if (highlightEffect != null)
            highlightEffect.enabled = false;
        StartCoroutine(RemoveAfterDelay());
    }
    /// <summary>
    /// Deactivates looting (not used).
    /// </summary>
    public new void Deactivate() { }
    /// <summary>
    /// Calls Activate() for legacy compatibility.
    /// </summary>
    protected override void ActivateComponent()
    {
        Activate();
    }
    /// <summary>
    /// Calls Deactivate() for legacy compatibility.
    /// </summary>
    protected override void DeactivateComponent()
    {
        Deactivate();
    }
    /// <summary>
    /// Switch not used for looting.
    /// </summary>
    public new void Switch() { }
    /// <summary>
    /// Coroutine to remove enemy after looting.
    /// </summary>
    private IEnumerator RemoveAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
