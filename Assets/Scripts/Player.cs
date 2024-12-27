using System;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public ParticleSystem dust;
    public ParticleSystem fire;

    public float speed;
    public float defaultSpeed;
    
    public float dashSpeed;
    public float dashRecharge;
    public bool inDash;

    public float jumpForce;
    public bool isJumping;
    public bool doubleJumping;
    public float jumpExtraGravity;

    private Rigidbody2D rig;
    private Animator anim;
    public int munition;

    public GameObject bullet;
    public float direction;
    public Transform gun;
    public float shotForce;
    public float megaShotForce;

    public bool isBurned;
    public float burnCooldown;

    public int life;
    public int maxLife;
    public Image[] coracao;
    public GameObject blow;

    public SpriteRenderer sr;
    public BoxCollider2D box;
    public CircleCollider2D circle;

    public static Player Instance;
    
    void Start()
    {
        defaultSpeed = speed;
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        circle = GetComponent<CircleCollider2D>();
        box = GetComponent<BoxCollider2D>();
        Instance = this;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 dir = new Vector2(x, y);

        Move(dir);
        jump();
        shoot();
        Dash();
        burn();
        gravityJump();
        Burned();
        HealthLogic();
        overCharge();

    }

    void HealthLogic()
    {
        for (int i = 0; i < coracao.Length; i++)
        {
            if (i < life)
            {
                coracao[i].enabled = true;
            }
            else
            {
                coracao[i].enabled = false;
            }
        }

        if(life == 0)
        {
            sr.enabled = false;
            blow.SetActive(true);

            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            Invoke("restart", 0.6f);
        }
    }

    void restart()
    {
        SceneManager.LoadScene("Scene 1");
    }

    void applyJumpExtraGravity()
    {
        Vector2 velocity = this.rig.linearVelocity;
        if(velocity.y > 0)
        {
            Vector2 extraGravity = (this.jumpExtraGravity * Vector2.down);
            this.rig.AddForce(extraGravity, ForceMode2D.Force);
        }
        
    }

    void gravityJump()
    {
        if (this.isJumping)
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                applyJumpExtraGravity();
            }
        }
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

    void Move(Vector2 dir)
    {

        

        rig.linearVelocity = new Vector2(dir.x * speed, rig.linearVelocityY);

        if (Input.GetButtonDown("Horizontal"))
        {
            CreateDust();
        }

        if (dir.x > 0)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 180f, 0f);

        }
        else if (dir.x < 0)
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
                rig.linearVelocity = new Vector2(rig.linearVelocityX, 0);
                rig.linearVelocity += Vector2.up * jumpForce;
                doubleJumping = true;
                anim.SetBool("jump", true);
                CreateDust();
                if (rig.linearVelocity.y < 0)
                {
                    jumpForce -= rig.linearVelocity.y;
                }
            }
            else
            {
                if (doubleJumping)
                {
                    rig.linearVelocity = new Vector2(rig.linearVelocityX, 0);
                    rig.linearVelocity += Vector2.up * jumpForce;
                    doubleJumping = false;
                    CreateDust();

                }
            }
        }
    }

    void shoot()
{
    if (Input.GetButtonDown("Fire1"))
    {
            if (munition > 0 && !isBurned )
        {

                GameObject temp = Instantiate(bullet);
                anim.SetTrigger("fire");
                temp.transform.position = gun.position;
                direction = (transform.eulerAngles.y == 180) ? 1 : -1;
                temp.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shotForce * direction, 0f);
                temp.GetComponent<Bullet>().damage = munition;
                temp.GetComponent<Bullet>().shooter = 1;

                if (munition == 3)
                {
                    temp.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(megaShotForce * direction, 0f);
                    temp.transform.localScale = new Vector3(0.24f, 0.14f, 0.189f);
                    if (ColorUtility.TryParseHtmlString("#FFBDD6", out Color newColor))
                    {
                        temp.GetComponent<SpriteRenderer>().color = newColor;
                    }
                }
                else
                {
                    temp.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(2 * shotForce * direction, 0f);
                }
            
            munition = 0;
            Destroy(temp.gameObject, 3f);

            }
        }
}
    void Burned()
    {
        if(munition == 4)
        {
            isBurned = true;
        }
    }

    void burn()
    {
        if (isBurned)
        {
            munition = 2;
            anim.SetTrigger("burn");
            Invoke("burnDelay", burnCooldown);
        }
    }

    void burnDelay()
    {
        isBurned = false;
        anim.ResetTrigger("burn");
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

    void CreateDust()
    {
        dust.Play();
        
    }

    void DisableDust()
    {
        dust.Stop();
    }

    void overCharge()
    {
        if(munition == 3 && !fire.isPlaying)
        {
            fire.Play();
        }
    }
    
}
