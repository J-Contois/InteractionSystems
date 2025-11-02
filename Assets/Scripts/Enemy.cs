using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls enemy health, death, and looting logic. Handles health bar visuals and death behavior.
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Maximum health of the enemy.")]
    [SerializeField] private float maxHealth = 100f;
    [Tooltip("Current health of the enemy.")]
    [SerializeField] private float currentHealth = 100f;
    [Header("UI")]
    [Tooltip("Slider for health bar.")]
    [SerializeField] private Slider healthBar = null;
    [Tooltip("Image for health bar fill.")]
    [SerializeField] private Image healthBarFill = null;
    [Tooltip("Color when health is full.")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [Tooltip("Color when health is low.")]
    [SerializeField] private Color lowHealthColor = Color.red;
    [Header("Ragdoll")]
    [Tooltip("Animator for ragdoll/death animation.")]
    [SerializeField] private Animator animator = null;
    [Tooltip("Main collider for the enemy.")]
    [SerializeField] private Collider mainCollider = null;
    [Header("Loot Toggle")]
    [Tooltip("Reference to the loot toggle component.")]
    [SerializeField] private EnemyLootToggle lootToggle = null;

    private float damageImmunityTime = 0.1f; // Time in seconds to be immune after taking damage
    private float lastDamageTime = -999f;

    /// <summary>
    /// Initializes health bar and disables highlight on start.
    /// </summary>
    private void Awake()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        UpdateHealthBar();
        if (lootToggle != null && lootToggle.highlightEffect != null)
            lootToggle.highlightEffect.enabled = false;
    }

    /// <summary>
    /// Applies damage to the enemy, triggers death if health reaches zero.
    /// </summary>
    public void TakeDamage(float amount)
    {
        // Prevent double damage from the same bullet passing through multiple colliders
        if (Time.time - lastDamageTime < damageImmunityTime) return;
        lastDamageTime = Time.time;
        Debug.Log("Enemy took damage: " + amount);
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        UpdateHealthBar();
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Updates the health bar visuals and color.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
        // Update fill color based on health percentage
        if (healthBarFill != null)
        {
            float t = Mathf.InverseLerp(0f, maxHealth, currentHealth);
            healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, t);
        }
    }

    /// <summary>
    /// Handles enemy death: disables health bar, disables animator/collider, lays flat, enables looting.
    /// </summary>
    private void Die()
    {
        // Hide health bar
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
        // Disable animator and main collider
        if (animator != null)
            animator.enabled = false;
        if (mainCollider != null)
            mainCollider.enabled = false;
        // Lay the enemy flat on the ground
        var rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(-90f, rot.y, rot.z); // Rotate so capsule lies flat
        // Optionally, move enemy slightly down to rest on ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f))
        {
            transform.position = hit.point + new Vector3(0f, 0.5f, 0f); // Adjust height as needed
        }
        // Enable looting via EnemyLootToggle
        if (lootToggle != null)
            lootToggle.EnableLoot();
    }
}
