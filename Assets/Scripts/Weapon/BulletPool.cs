using UnityEngine;
using System.Collections.Generic;

namespace Weapon
{
    /// <summary>
    /// Manages a pool of reusable bullet objects to optimize performance.
    /// </summary>
    public class BulletPool : MonoBehaviour
    {
        /// <summary>Prefab used to instantiate new bullets.</summary>
        [SerializeField] private GameObject bulletPrefab;
        /// <summary>Initial number of bullets in the pool.</summary>
        [SerializeField] private int poolSize = 20;
        /// <summary>Internal list of pooled bullet objects.</summary>
        private List<BulletBehavior> _pool;

        /// <summary>
        /// Initializes the bullet pool by instantiating bullet objects.
        /// </summary>
        private void Awake()
        {
            _pool = new List<BulletBehavior>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                var bulletObj = Instantiate(bulletPrefab, transform);
                bulletObj.SetActive(false);
                var bullet = bulletObj.GetComponent<BulletBehavior>();
                _pool.Add(bullet);
            }
        }

        /// <summary>
        /// Retrieves an inactive bullet from the pool, or instantiates a new one if needed.
        /// </summary>
        /// <returns>A BulletBehavior instance ready for use.</returns>
        public BulletBehavior GetBullet()
        {
            foreach (var bullet in _pool)
            {
                if (!bullet.gameObject.activeInHierarchy)
                    return bullet;
            }
            // Optionally expand pool or reuse first
            var bulletObj = Instantiate(bulletPrefab, transform);
            bulletObj.SetActive(false);
            var newBullet = bulletObj.GetComponent<BulletBehavior>();
            _pool.Add(newBullet);
            return newBullet;
        }
    }
}
