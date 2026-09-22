using UnityEngine;

public class BannerProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private AudioClip hitSound;

    private Rigidbody2D projectileRigidbody;

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody2D>();
        projectileRigidbody.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth == null)
        {
            return;
        }

        PlaySound(hitSound, other.transform.position);
        GameFlowController.Instance?.RegisterEnemyHit();
        enemyHealth.TakeDamage(damage);
        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }
}