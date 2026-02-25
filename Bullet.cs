using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float bulletSpeed = 1000f;
    public float maxLifetime = 3f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction)
    {
        _rb.AddForce(direction * bulletSpeed);
        
        Destroy(gameObject, maxLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
