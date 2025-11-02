using Core;
using UnityEngine;
using System.Collections;

public class EnemyLootToggle : BaseToggleComponent
{
    [Header("Highlight")]
    [SerializeField] public Behaviour highlightEffect;
    private bool canLoot = false;
    private bool isLooted = false;

    public void EnableLoot()
    {
        canLoot = true;
        if (highlightEffect != null)
            highlightEffect.enabled = true;
    }

    public new void Activate()
    {
        Debug.Log("Enabling loot");
        if (!canLoot || isLooted) return;
        isLooted = true;
        canLoot = false;
        if (highlightEffect != null)
            highlightEffect.enabled = false;
        StartCoroutine(RemoveAfterDelay());
        Debug.Log("Done looting");
    }
    public new void Deactivate() { }
    protected override void ActivateComponent()
    {
        Activate();
    }

    protected override void DeactivateComponent()
    {
        Deactivate();
    }

    public new void Switch() { }

    private IEnumerator RemoveAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}

