using UnityEngine;

/// <summary>
/// Generic enemy bullet (soldier). Moves forward and damages player.
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 10f;
    public float lifeTime = 4f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Prevent infinite travel when hitting environment
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
