using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [Header("General")]
        public string weaponName;
        [Header("Stats")]
        public float fireRate = 5f;
        public int maxAmmo = 30;
        public float reloadTime = 1.5f;
        public float damage = 25f;
        [Header("FOV")]
        public float aimFOV = 40f;
        public float normalFOV = 60f;
        [Header("Recoil & Spread")]
        public float recoilAmount = 2f;
        public float recoilRecovery = 8f;
        public float minSpreadAngle = 0.1f;
        public float autoSpreadAngle = 3f;
    }
}
