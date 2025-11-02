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
        [SerializeField] private TMP_Text ammoText;
        [SerializeField] private TMP_Text weaponText;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private GameObject generalCrosshairUI;
        [SerializeField] private GameObject weaponCrosshairUI;

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

        private void OnWeaponActiveChanged(bool active)
        {
            SetWeaponUIVisible(active);
            UpdateCrosshairVisibility(active);
        }

        private void UpdateCrosshairVisibility(bool weaponActive)
        {
            if (generalCrosshairUI != null)
                generalCrosshairUI.SetActive(!weaponActive);
            if (weaponCrosshairUI != null)
                weaponCrosshairUI.SetActive(weaponActive);
        }

        public void UpdateAmmo(int current, int max)
        {
            SetWeaponUIVisible(true);
            if (ammoText != null)
                ammoText.text = $"Ammo: {current} / {max}";
        }

        public void UpdateWeapon(string weaponName)
        {
            SetWeaponUIVisible(true);
            if (weaponText != null)
                weaponText.text = $"Weapon: {weaponName}";
        }

        public void UpdateStamina(float normalized)
        {
            if (staminaSlider != null)
                staminaSlider.value = normalized;
        }

        private void SetWeaponUIVisible(bool visible)
        {
            if (ammoText != null)
                ammoText.gameObject.SetActive(visible);
            if (weaponText != null)
                weaponText.gameObject.SetActive(visible);
        }
    }
}
