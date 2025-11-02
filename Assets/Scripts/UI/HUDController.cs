using UnityEngine;
using UnityEngine.UI;
using Weapon;
using Player;
using TMPro;

namespace UI
{
    /// <summary>
    /// Controls the HUD elements: ammo, weapon info, stamina, etc.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Text for displaying current ammo.")]
        [SerializeField] private TMP_Text ammoText = null;
        [Tooltip("Text for displaying weapon name.")]
        [SerializeField] private TMP_Text weaponText = null;
        [Tooltip("Slider for stamina bar.")]
        [SerializeField] private Slider staminaSlider = null;
        [Tooltip("Reference to WeaponController.")]
        [SerializeField] private WeaponController weaponController = null;
        [Tooltip("Reference to PlayerMovement.")]
        [SerializeField] private PlayerMovement playerMovement = null;
        [Tooltip("General crosshair UI GameObject.")]
        [SerializeField] private GameObject generalCrosshairUI = null;
        [Tooltip("Weapon crosshair UI GameObject.")]
        [SerializeField] private GameObject weaponCrosshairUI = null;

        /// <summary>
        /// Initializes HUD and crosshair visibility on start.
        /// </summary>
        private void Start()
        {
            if (weaponController != null)
            {
                weaponController.onAmmoChanged.AddListener(UpdateAmmo);
                weaponController.onWeaponChanged.AddListener(UpdateWeapon);
                weaponController.onWeaponActiveChanged.AddListener(OnWeaponActiveChanged);
            }
            if (playerMovement != null)
            {
                playerMovement.onStaminaChanged.AddListener(UpdateStamina);
            }
            SetWeaponUIVisible(weaponController != null && weaponController.HasWeapon());
            UpdateCrosshairVisibility(weaponController != null && weaponController.HasWeapon() && weaponController.IsWeaponEquipped());
        }

        /// <summary>
        /// Called when the active state of the weapon changes.
        /// </summary>
        private void OnWeaponActiveChanged(bool active)
        {
            SetWeaponUIVisible(active);
            UpdateCrosshairVisibility(active);
        }

        /// <summary>
        /// Updates the visibility of the crosshair based on weapon activity.
        /// </summary>
        private void UpdateCrosshairVisibility(bool weaponActive)
        {
            if (generalCrosshairUI != null)
                generalCrosshairUI.SetActive(!weaponActive);
            if (weaponCrosshairUI != null)
                weaponCrosshairUI.SetActive(weaponActive);
        }

        /// <summary>
        /// Updates ammo display.
        /// </summary>
        public void UpdateAmmo(int current, int max)
        {
            SetWeaponUIVisible(true);
            if (ammoText != null)
                ammoText.text = $"Ammo: {current} / {max}";
        }

        /// <summary>
        /// Updates weapon name display.
        /// </summary>
        public void UpdateWeapon(string weaponName)
        {
            SetWeaponUIVisible(true);
            if (weaponText != null)
                weaponText.text = $"Weapon: {weaponName}";
        }

        /// <summary>
        /// Updates stamina bar display.
        /// </summary>
        public void UpdateStamina(float normalized)
        {
            if (staminaSlider != null)
                staminaSlider.value = normalized;
        }

        /// <summary>
        /// Sets visibility of weapon UI elements.
        /// </summary>
        private void SetWeaponUIVisible(bool visible)
        {
            if (ammoText != null)
                ammoText.gameObject.SetActive(visible);
            if (weaponText != null)
                weaponText.gameObject.SetActive(visible);
        }
    }
}
