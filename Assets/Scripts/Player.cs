using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private Rigidbody2D rig;
    public bool isJumping;
    public bool doubleJumping;
    private Animator anim;
    public int munition;

    public GameObject bullet;
    public Transform gun;
    private bool shot;
    public float shotForce;
    private bool flipX = false;

    public static Player Instance;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        Instance = this;
    }

    void Update()
    {
        Move();
        jump();
        shoot();
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * speed;
        if (Input.GetAxis("Horizontal") > 0)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 180f, 0f); 
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 0f, 0f); 
        }
        else
        {
            anim.SetBool("walk", false);
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!isJumping)
            {
                rig.AddForce(new Vector2(0f, jumpForce),ForceMode2D.Impulse);
                doubleJumping = true;
                anim.SetBool("jump", true);
            }
            else
            {
                if (doubleJumping)
                {
                    rig.AddForce(new Vector3(0f, jumpForce), ForceMode2D.Impulse);
                    doubleJumping = false;

                }
            }
        }
    }

    void shoot()
{
    if (Input.GetButtonDown("Fire1"))
    {
        if (munition > 0)
        {
            anim.SetTrigger("fire");
            GameObject temp = Instantiate(bullet);
            temp.transform.position = gun.position;

           
            float direction = (transform.eulerAngles.y == 180) ? 1 : -1;
            temp.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shotForce * direction, 0f);
            Destroy(temp.gameObject, 3f);
            munition--;
        }
    }
}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isJumping = false;
            anim.SetBool("jump", false);
        }
        
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isJumping = true;
        }
    }
}