using UnityEngine;

namespace Weapon
{
    /// <summary>
    /// Handles weapon pickup logic. Stores reference to WeaponData for the picked weapon.
    /// </summary>
    public class WeaponPickup : MonoBehaviour
    {
        /// <summary>Reference to the weapon's data (ScriptableObject).</summary>
        public WeaponData weaponData;
    }
}
