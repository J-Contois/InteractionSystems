using UnityEngine;

namespace Weapon
{
    /// <summary>
    /// Controls bullet movement, lifetime, and deactivation.
    /// </summary>
    public class BulletBehavior : MonoBehaviour
    {
        /// <summary>Speed of the bullet in units per second.</summary>
        [SerializeField] private float _speed = 30f;
        /// <summary>Lifetime of the bullet in seconds before deactivation.</summary>
        [SerializeField] private float _lifeTime = 2f;
        /// <summary>Current movement direction of the bullet.</summary>
        private Vector3 _direction;
        /// <summary>Timer tracking bullet's lifetime.</summary>
        private float _timer;
        /// <summary>Whether the bullet is currently active.</summary>
        private bool _active;

        private Rigidbody _rigidbody;

        [Header("Hit Effects")]
        [Tooltip("Prefab for hit effect (particle)")]
        [SerializeField] private GameObject hitEffectPrefab;
        [Tooltip("Audio clip for hit sound")]
        [SerializeField] private AudioClip hitSound;
        private AudioSource _audioSource;
        [Tooltip("Reference to WeaponController for screen shake")]
        [SerializeField] private WeaponController weaponController;
        public void SetWeaponController(WeaponController controller)
        {
            weaponController = controller;
        }

        /// <summary>
        /// Activates the bullet and sets its movement direction.
        /// </summary>
        /// <param name="direction">Direction to move the bullet.</param>
        public void Fire(Vector3 direction)
        {
            _direction = direction.normalized;
            _timer = 0f;
            _active = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Moves the bullet and checks for lifetime expiration.
        /// </summary>
        private void Update()
        {
            if (!_active) return;
            transform.position += _direction * (_speed * Time.deltaTime);
            _timer += Time.deltaTime;
            if (_timer >= _lifeTime)
                Deactivate();
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogWarning("BulletBehavior: No Rigidbody found. Adding one for trigger events.");
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            // Ensure bullet is not affected by gravity or physics forces
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        /// <summary>
        /// Deactivates the bullet on collision.
        /// </summary>
        /// <param name="other">Collider that was hit.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (hitEffectPrefab)
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            if (hitSound && _audioSource)
                _audioSource.PlayOneShot(hitSound);
            if (weaponController)
                weaponController.OnBulletHit();
            // Damage enemy if hit (supports multi-collider enemies)
            var enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
            }
            Deactivate();
        }

        /// <summary>
        /// Deactivates the bullet and hides it from the scene.
        /// </summary>
        public void Deactivate()
        {
            _active = false;
            gameObject.SetActive(false);
        }

        private float _damage;
        public void SetDamage(float damage)
        {
            _damage = damage;
        }
    }
}
