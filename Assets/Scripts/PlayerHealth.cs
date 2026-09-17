using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Transform heartsContainer;
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(maxHealth, 1, GetHeartCount());
        UpdateHearts();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<CannonballSpawner>(out _))
        {
            return;
        }

        TakeDamage(1);
        Destroy(other.gameObject);
    }

    private void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHearts();

        if (currentHealth == 0)
        {
            Debug.Log("Player defeated.", this);
            GameFlowController.Instance?.ShowDefeat();
            enabled = false;
        }
    }

    private int GetHeartCount()
    {
        if (heartsContainer == null)
        {
            Debug.LogError("Assign the hearts container to PlayerHealth.", this);
            return maxHealth;
        }

        return heartsContainer.childCount;
    }

    private void UpdateHearts()
    {
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