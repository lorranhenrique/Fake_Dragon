using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public float speed;
    public float defaultSpeed;
    
    public float dashSpeed;
    public float dashRecharge;
    public bool inDash;

    public float jumpForce;
    public bool isJumping;
    public bool doubleJumping;

    private Rigidbody2D rig;
    private Animator anim;
    public int munition;

    public GameObject bullet;
    public Transform gun;
    private bool shot;
    public float shotForce;

    public bool isBurned;
    public float burnCooldown;


    private bool flipX = false;

    public static Player Instance;
    
    void Start()
    {
        defaultSpeed = speed;
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        Instance = this;
    }

    void Update()
    {
        Move();
        jump();
        shoot();
        Dash();
        burn();
    }

    void Dash()
    {
        if (Input.GetButtonDown("Fire2") && inDash==false)
        {
            defaultSpeed = speed; 
            speed = dashSpeed;

            if (isJumping)
            {
                isJumping = false;
                anim.SetBool("jump", false);
            }
            if (Input.GetAxis("Horizontal") != 0)
            {
                anim.SetTrigger("dash");
            }
            
            inDash = true;
            Invoke("posDash", 0.1f);
        }
     
    }

    void posDash()
    {
        speed = defaultSpeed;
        Invoke("dashEnd", dashRecharge);

    }

    void dashEnd()
    {
        inDash = false;
    }

    void Move()
    {

        float movement = Input.GetAxis("Horizontal");

        rig.linearVelocity = new Vector2(movement * speed, rig.linearVelocityY);

        if (movement > 0)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 180f, 0f); 
        }
        else if (movement < 0)
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
        if (munition > 0 && !isBurned)
        {
            munition--;
            anim.SetTrigger("fire");
            GameObject temp = Instantiate(bullet);

            temp.transform.position = gun.position;

           
            float direction = (transform.eulerAngles.y == 180) ? 1 : -1;
            temp.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shotForce * direction, 0f);
            Destroy(temp.gameObject, 3f);
            
        }
    }
}

    void burn()
    {
        if (isBurned)
        {
            munition = 2;
            anim.SetBool("burn",true);
            Invoke("burnDelay", burnCooldown);
        }
    }
    void burnDelay()
    {
        isBurned = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isJumping = false;
            anim.SetBool("jump", false);
            //speed = defaultSpeed;
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
