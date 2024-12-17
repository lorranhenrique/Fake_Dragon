using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject blow; // Objeto com a animação de explosão
    private CircleCollider2D circle;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    void Start()
    {
        circle = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == 8) // Layer do chão ou muro
        {
            
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

           
            circle.enabled = false;
            sr.enabled = false;

            
            blow.SetActive(true);

            
            Destroy(gameObject, 0.6f);
        }
    }
}
