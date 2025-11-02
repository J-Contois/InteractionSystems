using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [Header("UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthBarFill; // Reference to the fill image
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [Header("Ragdoll")]
    [SerializeField] private GameObject ragdollRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider mainCollider;
    [Header("Loot Toggle")]
    [SerializeField] private EnemyLootToggle lootToggle;

    private float damageImmunityTime = 0.1f; // Time in seconds to be immune after taking damage
    private float lastDamageTime = -999f;

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
            transform.position = hit.point +  new Vector3(0f, 0.5f, 0f); // Adjust height as needed
        }
        // Enable looting via EnemyLootToggle
        if (lootToggle != null)
            lootToggle.EnableLoot();
    }
}
