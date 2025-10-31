using UnityEngine;
using Core;

namespace Toggles.Components
{
    /// <summary>
    /// Component that simply enables/disables a target GameObject.
    /// Used for invisible walls, lights, particle effects, or any simple on/off objects.
    /// </summary>
    public class GameObjectToggleComponent : BaseToggleComponent
    {
        [Header("Target Settings")]
        [Tooltip("The GameObject to enable/disable")]
        [SerializeField] private GameObject targetObject = null;

        [Tooltip("If true, activates the target. If false, deactivates it.")]
        [SerializeField] private bool activateOnActivate = true;

        private void Reset()
        {
            if (targetObject == null)
                targetObject = gameObject;
        }

        protected override void ActivateComponent()
        {
            if (targetObject != null)
            {
                targetObject.SetActive(activateOnActivate);
                Debug.Log($"{gameObject.name} - Setting {targetObject.name} active: {activateOnActivate}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - No target GameObject assigned!");
            }
        }

        protected override void DeactivateComponent()
        {
            if (targetObject != null)
            {
                targetObject.SetActive(!activateOnActivate);
                Debug.Log($"{gameObject.name} - Setting {targetObject.name} active: {!activateOnActivate}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} - No target GameObject assigned!");
            }
        }
    }
}