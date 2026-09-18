using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject cannonball;
    public Transform cannonballPos;
    [SerializeField] private AudioClip shootSound;

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 2f)
        {
            timer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, cannonballPos.position);
        }

        Instantiate(cannonball, cannonballPos.position, Quaternion.identity);
    }
}
