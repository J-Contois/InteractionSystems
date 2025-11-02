using UnityEngine;

namespace Weapon
{
    /// <summary>
    /// ScriptableObject containing all configurable weapon stats and settings.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [Header("General")]
        [Tooltip("Name of the weapon.")]
        public string weaponName = "Default Weapon";
        [Header("Stats")]
        [Tooltip("Weapon fire rate (shots per second).")]
        public float fireRate = 5f;
        [Tooltip("Maximum ammo in magazine.")]
        public int maxAmmo = 30;
        [Tooltip("Reload time in seconds.")]
        public float reloadTime = 1.5f;
        [Tooltip("Damage dealt per shot.")]
        public float damage = 25f;
        [Header("FOV")]
        [Tooltip("Field of view when aiming.")]
        public float aimFOV = 40f;
        [Tooltip("Field of view when not aiming.")]
        public float normalFOV = 60f;
        [Header("Recoil & Spread")]
        [Tooltip("Amount of camera recoil per shot.")]
        public float recoilAmount = 2f;
        [Tooltip("Speed at which recoil recovers.")]
        public float recoilRecovery = 8f;
        [Tooltip("Minimal spread angle when aiming or single shot.")]
        public float minSpreadAngle = 0.1f;
        [Tooltip("Spread angle for full auto fire.")]
        public float autoSpreadAngle = 3f;
    }
}
