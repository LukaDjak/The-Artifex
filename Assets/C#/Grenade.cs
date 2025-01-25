using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosionEffect; // Reference to explosion particle effect
    public float explosionRadius = 5f; // Explosion radius
    public float explosionForce = 10f; // Explosion force applied to nearby objects
    public float delay = 3f; // Delay before explosion

    private void Start() => Invoke(nameof(Explode), delay);

    private void OnCollisionEnter2D(Collision2D collision) => Explode(); // Explode when colliding with any object
    
    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var collider in colliders)
        {
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (collider.transform.position - transform.position).normalized;
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
            }

            if (collider.CompareTag("Enemy"))
                collider.GetComponent<Enemy>().TakeDamage(75);
        }

        Destroy(gameObject); //destroy the grenade after it explodes
    }
}
