using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Transform heartsContainer;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private AudioClip hitSound;

    private int currentHealth;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(maxHealth, 1, GetHeartCount());
        UpdateHearts();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check for collisions with objects that can damage the player
        if (other.gameObject.CompareTag("Cannonball"))
        {
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, other.transform.position);
            }

            TakeDamage(1);
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("KillZone"))
        {
            TakeDamage(currentHealth);
        }
    }

    private void TakeDamage(int damage)
    {
        // Reduce the player's health and update the heart display
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHearts();

        // Check if the player has been defeated
        if (currentHealth == 0)
        {
            Debug.Log("Player defeated.", this);
            GameFlowController.Instance?.ShowDefeat(currentHealth);
            enabled = false;
        }
    }

    private int GetHeartCount()
    {
        // Return the number of heart GameObjects in the hearts container
        if (heartsContainer == null)
        {
            Debug.LogError("Assign the hearts container to PlayerHealth.", this);
            return maxHealth;
        }

        return heartsContainer.childCount;
    }

    private void UpdateHearts()
    {
        // Update the heart display based on the current health
        if (heartsContainer == null)
        {
            return;
        }

        for (int index = 0; index < heartsContainer.childCount; index++)
        {
            heartsContainer.GetChild(index).gameObject.SetActive(index < currentHealth);
        }
    }
}