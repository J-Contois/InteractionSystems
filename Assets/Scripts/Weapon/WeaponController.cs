using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    /// <summary>
    /// Handles weapon equip/unequip, shooting, aiming, and crosshair display.
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the Player script")]
        [SerializeField] private Player.Player player = null;
        [Tooltip("Reference to the crosshair UI GameObject")]
        [SerializeField] private GameObject crosshairUI = null;
        [Tooltip("Reference to the player's camera")]
        [SerializeField] private Camera playerCamera = null;

        [Header("Weapon Data")]
        [Tooltip("Current weapon data")]
        [SerializeField] private WeaponData weaponData = null;
        [Header("Weapon References")]
        [Tooltip("Prefab for the bullet/projectile")]
        [SerializeField] private GameObject bulletPrefab = null;
        [Tooltip("Transform for bullet spawn position")]
        [SerializeField] private Transform firePoint = null;
        [Tooltip("Bullet pool component")]
        [SerializeField] private BulletPool bulletPool = null;

        [Header("Screen Shake")]
        [Tooltip("Screen shake intensity on hit")]
        [SerializeField] private float hitShakeIntensity = 0.2f;
        [Tooltip("Screen shake duration on hit")]
        [SerializeField] private float hitShakeDuration = 0.15f;
        private float _shakeTimer = 0f;
        private float _shakeAmount = 0f;

        [Header("Spread Settings")]
        [Tooltip("Minimal spread angle when aiming or single shot")]
        [SerializeField] private float minSpreadAngle = 0.1f;
        [Tooltip("Spread angle for full auto fire")]
        [SerializeField] private float autoSpreadAngle = 3f;
        private float _fireHoldTimer = 0f;
        private bool _isFiring = false;
        private const float fullAutoThreshold = 0.2f;
        
        [Header("Weapon Settings (Fallback)")]
        [Tooltip("Maximum ammo in magazine (fallback)")]
        [SerializeField] private int maxAmmo = 30;
        [Tooltip("Reload time in seconds (fallback)")]
        [SerializeField] private float reloadTime = 1.5f;
        [Tooltip("Camera recoil amount in degrees (fallback)")]
        [SerializeField] private float recoilAmount = 2f;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent<int, int> onAmmoChanged;
        public UnityEngine.Events.UnityEvent<string> onWeaponChanged;
        public UnityEngine.Events.UnityEvent<bool> onWeaponActiveChanged;
        
        private int _currentAmmo = 0;
        private bool _isReloading = false;
        private float _reloadTimer = 0f;

        /// <summary>Time until next allowed shot.</summary>
        private float _nextFireTime = 0f;
        /// <summary>Whether the player is currently aiming.</summary>
        private bool _isAiming = false;
        /// <summary>Whether the player has picked up a weapon.</summary>
        private bool _hasWeapon = false;
        /// <summary>Whether the weapon is currently equipped.</summary>
        private bool _isWeaponEquipped = false;

        private Quaternion _originalCameraRotation = Quaternion.identity;
        private float _recoilOffset = 0f;

        // Stores runtime data for each weapon by name
        private Dictionary<string, int> weaponAmmoStates = new();

        /// <summary>
        /// Unity Update loop. Handles input for weapon actions and crosshair display.
        /// </summary>
        private void Update()
        {
            // Equip/unequip weapon (E/U)
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (HasWeapon())
                    EquipWeapon();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                if (HasWeapon())
                    UnequipWeapon();
            }

            // Show/hide crosshair based on weapon state
            if (crosshairUI)
                crosshairUI.SetActive(HasWeapon() && IsWeaponEquipped());

            // Aiming (right mouse button)
            if (HasWeapon() && IsWeaponEquipped())
            {
                if (Input.GetMouseButtonDown(1))
                    StartAiming();
                if (Input.GetMouseButtonUp(1))
                    StopAiming();

                // Shooting (left mouse button)
                if (Input.GetMouseButton(0) && Time.time >= _nextFireTime)
                {
                    Shoot();
                    _nextFireTime = Time.time + 1f / weaponData.fireRate;
                }
            }

            // Track fire button hold duration
            if (HasWeapon() && IsWeaponEquipped())
            {
                if (Input.GetMouseButton(0))
                {
                    _fireHoldTimer += Time.deltaTime;
                    _isFiring = true;
                }
                else
                {
                    _fireHoldTimer = 0f;
                    _isFiring = false;
                }
            }

            HandleRecoil();
            HandleReload();
            HandleScreenShake();
            // Reload (R key)
            if (Input.GetKeyDown(KeyCode.R) && !_isReloading && _currentAmmo < weaponData.maxAmmo)
                StartReload();
        }

        /// <summary>
        /// Returns true if the player has picked up a weapon.
        /// </summary>
        public bool HasWeapon() => _hasWeapon;
        /// <summary>
        /// Returns true if the weapon is currently equipped.
        /// </summary>
        public bool IsWeaponEquipped() => _isWeaponEquipped;
        /// <summary>
        /// Equips the weapon if the player has one.
        /// </summary>
        private void EquipWeapon()
        {
            if (_hasWeapon) {
                _isWeaponEquipped = true;
                onWeaponActiveChanged?.Invoke(true);
            }
        }
        /// <summary>
        /// Unequips the weapon.
        /// </summary>
        private void UnequipWeapon()
        {
            _isWeaponEquipped = false;
            StopAiming();
            onWeaponActiveChanged?.Invoke(false);
        }
        /// <summary>
        /// Starts aiming (changes camera FOV).
        /// </summary>
        private void StartAiming()
        {
            _isAiming = true;
            if (playerCamera)
                playerCamera.fieldOfView = weaponData != null ? weaponData.aimFOV :  weaponData.normalFOV;
        }
        /// <summary>
        /// Stops aiming (restores camera FOV).
        /// </summary>
        private void StopAiming()
        {
            _isAiming = false;
            if (playerCamera)
                playerCamera.fieldOfView = weaponData != null ? weaponData.normalFOV :  weaponData.aimFOV;
        }
        /// <summary>
        /// Handles camera recoil recovery over time.
        /// </summary>
        private void HandleRecoil()
        {
            if (!playerCamera) return;
            // Lerp recoil offset back to zero
            if (Mathf.Abs(_recoilOffset) > 0.001f)
            {
                _recoilOffset = Mathf.Lerp(_recoilOffset, 0f, weaponData.recoilRecovery * Time.deltaTime);
            }
            // Apply recoil offset additively to original rotation
            playerCamera.transform.localRotation = _originalCameraRotation * Quaternion.Euler(-_recoilOffset, 0f, 0f);
        }
        /// <summary>
        /// Fires a bullet in the direction of the crosshair (center of screen), with spread and recoil.
        /// </summary>
        private void Shoot()
        {
            if (!(HasWeapon() && IsWeaponEquipped())) return;
            if (_isReloading || _currentAmmo <= 0) return;
            if (bulletPool && firePoint && playerCamera)
            {
                var bullet = bulletPool.GetBullet();
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = firePoint.rotation * Quaternion.Euler(90f, 0f, 0f);
                bullet.SetWeaponController(this);
                bullet.SetDamage(GetWeaponDamage());
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                Vector3 targetPoint;
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                {
                    targetPoint = hit.point;
                }
                else
                {
                    targetPoint = playerCamera.transform.position + playerCamera.transform.forward * 1000f;
                }
                Vector3 shootDir = (targetPoint - firePoint.position).normalized;
                float spread = weaponData != null ? weaponData.minSpreadAngle : minSpreadAngle;
                if (!IsAiming)
                {
                    if (_isFiring && _fireHoldTimer > fullAutoThreshold)
                        spread = weaponData != null ? weaponData.autoSpreadAngle : autoSpreadAngle;
                }
                shootDir = GetSpreadDirection(shootDir, spread);
                bullet.Fire(shootDir);
                _currentAmmo--;
                onAmmoChanged?.Invoke(_currentAmmo, weaponData != null ? weaponData.maxAmmo : maxAmmo);
                _recoilOffset += weaponData != null ? weaponData.recoilAmount : recoilAmount;
            }
        }
        /// <summary>
        /// Returns a direction vector with random spread applied, tightly centered around the crosshair.
        /// </summary>
        /// <param name="baseDir">Base direction (camera forward)</param>
        /// <param name="maxAngle">Maximum spread angle in degrees</param>
        private Vector3 GetSpreadDirection(Vector3 baseDir, float maxAngle)
        {
            // Clamp maxAngle to a low value for tight spread
            float angle = Random.Range(0f, Mathf.Clamp(maxAngle, 0.1f, 3f));
            float azimuth = Random.Range(0f, 360f);
            // Rotate baseDir by 'angle' degrees around a random axis perpendicular to baseDir
            Quaternion spreadRot = Quaternion.AngleAxis(angle, Quaternion.AngleAxis(azimuth, baseDir) * Vector3.up);
            return spreadRot * baseDir;
        }

        /// <summary>
        /// Returns true if the player is currently aiming.
        /// </summary>
        public bool IsAiming => _isAiming;

        /// <summary>
        /// Handles reload timer and ammo refill.
        /// </summary>
        private void HandleReload()
        {
            if (_isReloading)
            {
                _reloadTimer += Time.deltaTime;
                float reloadTimeVal = weaponData != null ? weaponData.reloadTime : reloadTime;
                int maxAmmoVal = weaponData != null ? weaponData.maxAmmo : maxAmmo;
                if (_reloadTimer >= reloadTimeVal)
                {
                    _currentAmmo = maxAmmoVal;
                    _isReloading = false;
                    _reloadTimer = 0f;
                    onAmmoChanged?.Invoke(_currentAmmo, maxAmmoVal);
                }
            }
        }
        /// <summary>
        /// Starts the reload process.
        /// </summary>
        private void StartReload()
        {
            _isReloading = true;
            _reloadTimer = 0f;
        }

        /// <summary>
        /// Called by BulletBehavior when a bullet hits something. Triggers screen shake.
        /// </summary>
        public void OnBulletHit()
        {
            _shakeTimer = hitShakeDuration;
            _shakeAmount = hitShakeIntensity;
        }
        /// <summary>
        /// Handles screen shake effect.
        /// </summary>
        private void HandleScreenShake()
        {
            if (_shakeTimer > 0f && playerCamera)
            {
                _shakeTimer -= Time.deltaTime;
                float shake = _shakeAmount * Mathf.Sin(Time.time * 40f);
                playerCamera.transform.localPosition = new Vector3(0f, 0f, shake);
                if (_shakeTimer <= 0f)
                    playerCamera.transform.localPosition = Vector3.zero;
            }
        }

        // Returns the damage value for the current weapon
        public float GetWeaponDamage() => weaponData != null ? weaponData.damage : 25f;

        private void Awake()
        {
            int maxAmmoVal = weaponData != null ? weaponData.maxAmmo : maxAmmo;
            _currentAmmo = maxAmmoVal;
            onAmmoChanged?.Invoke(_currentAmmo, maxAmmoVal);
            onWeaponChanged?.Invoke(weaponData != null ? weaponData.weaponName : "DefaultWeapon");
            if (playerCamera)
                _originalCameraRotation = playerCamera.transform.localRotation;
            _recoilOffset = 0f;
        }
        public void OnWeaponPickedUp(GameObject weaponObject)
        {
            var data = weaponObject.GetComponent<WeaponPickup>()?.weaponData;
            if (data != null)
            {
                weaponData = data;
                int maxAmmoVal = weaponData.maxAmmo;
                // Restore ammo if previously used
                if (weaponAmmoStates.TryGetValue(weaponData.weaponName, out int savedAmmo))
                    _currentAmmo = savedAmmo;
                else
                    _currentAmmo = maxAmmoVal;
                onWeaponChanged?.Invoke(weaponData.weaponName);
                onAmmoChanged?.Invoke(_currentAmmo, maxAmmoVal);
            }
            _hasWeapon = true;
            EquipWeapon();
        }
        public void OnWeaponDropped(GameObject weaponObject)
        {
            // Save current ammo for this weapon
            if (weaponData != null)
                weaponAmmoStates[weaponData.weaponName] = _currentAmmo;
            _hasWeapon = false;
            _isWeaponEquipped = false;
            StopAiming();
            onWeaponActiveChanged?.Invoke(false);
            // Optionally clear references, update UI, etc.
        }
    }
}
