using UnityEngine;

public class BannerProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D projectileRigidbody;

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody2D>();
        projectileRigidbody.linearVelocity = Vector2.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth == null)
        {
            return;
        }

        enemyHealth.TakeDamage(damage);
        Destroy(gameObject);
    }
}