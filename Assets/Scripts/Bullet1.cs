using Unity.VisualScripting;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    public GameObject blow;
    private CircleCollider2D circle;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    public ParticleSystem smoke;
    public static Bullet1 Instance;
    public int damage;
    public float acelerator;
    public int atirador;


    void Start()
    {
        
        smoke.Play();
        circle = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Instance = this;
    }


    void animationTypeChange()
    {
        switch (damage)
        {
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            default:
                Debug.Log("Erro de disparo");
                break;
        }
    }

    private void Update()
    {
        if(atirador == 1)
        {
            rb.linearVelocity += new Vector2(acelerator * Time.deltaTime * Player.Instance.direction, 0f);
        }
        else
        {
            rb.linearVelocity += new Vector2(acelerator * Time.deltaTime * Player2.Instance.direction, 0f);
        }
        

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == 8) 
        {
            
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            acelerator = 0f;
            smoke.Stop();

           
            circle.enabled = false;
            sr.enabled = false;

            
            blow.SetActive(true);

            
            Destroy(gameObject, 0.5f);
        }
    }
}
