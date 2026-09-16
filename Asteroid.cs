using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Sprite[] sprites;
    public float size = 1.0f;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    public float speed = 50.0f;
    public float MaxLife = 100.0f;
    
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];

        transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f));
        transform.localScale = Vector3.one * size;

        rigidbody2D.mass = size * 2.5f;
    }

    public void SetTrajectory(Vector2 direction)
    {
        rigidbody2D.AddForce(direction * speed);

        Destroy(gameObject, MaxLife);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if ((size * 0.5f) >= minSize)
            {
                CreateSplit();
                CreateSplit();
            }
            
            GameEvents.AsteroidDestroyed(transform.position);
            Destroy(gameObject);
        }
    }

    private void CreateSplit()
    {
        Vector2 position = transform.position;
        position += UnityEngine.Random.insideUnitCircle * 0.5f;
            
        Asteroid half = Instantiate(this, position, transform.rotation);
        half.size = size * 0.5f;
        half.SetTrajectory(UnityEngine.Random.insideUnitCircle.normalized);
    }
    
    public DamageType GetDamageType()
    {
        return size > 1.0f ? DamageType.Full : DamageType.Half;
    }
}
