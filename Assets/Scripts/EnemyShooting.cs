using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject cannonball;
    public Transform cannonballPos;

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
        Instantiate(cannonball, cannonballPos.position, Quaternion.identity);
    }
}
