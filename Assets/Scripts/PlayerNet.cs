using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

public class PlayerNet : MonoBehaviourPunCallbacks
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

    public static PlayerNet Instance;

    private Player photonPlayer;
    private int id;

    public GameObject munitionText;
    public GameObject playerTag;

    public int playerNum;
    public bool jogoTerminado;

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
        if (!photonView.IsMine || jogoTerminado)
        {
            return;
        }

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
        AtualizaMunicao();

    }

    [PunRPC]
    void UpdateAnimationState(string animationState, bool value)
    {
        anim.SetBool(animationState, value);
    }

    [PunRPC]
    void UpdateAnimationTrigger(string triggerName)
    {
        if (anim != null)
        {
            anim.SetTrigger(triggerName);
            Debug.Log($"Trigger {triggerName} ativado.");
        }
        else
        {
            Debug.LogError("Animator não foi inicializado antes de SetTrigger.");
        }
    }
    
    [PunRPC]
    void UpdateFire(bool isplaying)
    {
        if (isplaying)
        {
            fire.Play();
        }
        else
        {
            fire.Stop();
        }
    }

    [PunRPC]
    void UpdateDust(bool isplaying)
    {

        if (isplaying)
        {
            dust.Play();
        }
        else
        {
            dust.Stop();
        }
    }

    [PunRPC]

    void UpdateColorTag(string newColor)
    {
        TMP_Text textTag = playerTag.GetComponent<TMP_Text>();
        if (ColorUtility.TryParseHtmlString("#FF0074", out Color Color) && playerNum == 2)
        {
            textTag.color = Color;
        }
    }

    [PunRPC]
    public void Inicialize(Player player)
    {
        playerNum = player.ActorNumber;

        if (playerTag != null)
        {
            TMP_Text textTag = playerTag.GetComponent<TMP_Text>();
            if (textTag != null)
            {
                textTag.text = $"P{playerNum}";
            }
        }

        if (playerNum == 2)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            photonView.RPC("UpdateColorTag", RpcTarget.AllBuffered, "#FF0074");
            photonView.RPC("SyncDirection", RpcTarget.All, transform.eulerAngles);
        }

        
        if (player == null)
        {
            Debug.LogError("Player passado para Inicialize é nulo!");
            return;
        }

        photonPlayer = player;
        id = player.ActorNumber;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Jogadores.Add(this);
        }
        else
        {
            Debug.LogError("GameManager.Instance é nulo ao inicializar jogador.");
        }

        if (!photonView.IsMine)
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void AtualizaMunicao()
    {
        if (munitionText != null)
        {
            TMP_Text textMeshPro = munitionText.GetComponent<TMP_Text>();
            if (textMeshPro != null)
            {
                textMeshPro.text = munition.ToString();
            }
        }
    }

    [PunRPC]
    void HealthLogic()
    {
        photonView.RPC("UpdateLife",RpcTarget.All);

        if (life <= 0)
        {
            photonView.RPC("UpdateDeath", RpcTarget.All);
        }
    }

    [PunRPC]
    void UpdateLife()
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
    }


    [PunRPC]
    void UpdateDeath()
    {
        sr.enabled = false;
        blow.SetActive(true);

        gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        GameManager.Instance.photonView.RPC("VerificaFimDeJogo", RpcTarget.All);

        photonView.RPC("updateGameState", RpcTarget.All);

        Invoke("score", 0.6f);
    }

    [PunRPC]

    void updateGameState()
    {
        jogoTerminado = true;
    }

    void score()
    {
        if (GameManager.Instance != null)
        {
            if (photonPlayer.IsLocal)
            {
                GameManager.Instance.placarDerrota.SetActive(true);
                GameManager.Instance.Botoes.SetActive(true);
            }
            else
            {
                GameManager.Instance.placarVitoria.SetActive(true);
                GameManager.Instance.Botoes.SetActive(true);
            }
        }
        //PhotonNetwork.LoadLevel("Scene 1");
    }

    void applyJumpExtraGravity()
    {
        Vector2 velocity = this.rig.linearVelocity;
        if (velocity.y > 0)
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
        if (Input.GetButtonDown("Fire2") && inDash == false)
        {
     
            defaultSpeed = speed;
            speed = dashSpeed;

            if (isJumping)
            {
                isJumping = false;
                anim.SetBool("jump", false);
                photonView.RPC("UpdateAnimationState", RpcTarget.Others, "jump", anim.GetBool("jump"));
            }
            if (Input.GetAxis("Horizontal") != 0)
            {
                anim.SetTrigger("dash");
                photonView.RPC("UpdateAnimationTrigger", RpcTarget.Others, "dash");
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

    [PunRPC]
    void SyncDirection(Vector3 direction)
    {
        transform.eulerAngles = direction;
    }

    void Move(Vector2 dir)
    {
        rig.linearVelocity = new Vector2(dir.x * speed, rig.linearVelocityY);

        photonView.RPC("UpdateAnimationState", RpcTarget.Others, "walk", anim.GetBool("walk"));

        if (Input.GetButtonDown("Horizontal"))
        {
            CreateDust();
            photonView.RPC("UpdateDust", RpcTarget.All, true);
        }

        if (dir.x > 0)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            photonView.RPC("SyncDirection", RpcTarget.Others, transform.eulerAngles);

        }
        else if (dir.x < 0)
        {
            anim.SetBool("walk", true);
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            photonView.RPC("SyncDirection", RpcTarget.Others, transform.eulerAngles);

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
                photonView.RPC("UpdateAnimationState", RpcTarget.Others, "jump", anim.GetBool("jump"));
                CreateDust();
                photonView.RPC("UpdateDust", RpcTarget.All, true);
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
                    photonView.RPC("UpdateDust", RpcTarget.All, true);

                }
            }
        }
    }

    [PunRPC]
    public void SetMunition(int newMunition)
    {
        
            munition = newMunition;
            AtualizaMunicaoNet();
        
    }
    
    public void AtualizaMunicaoNet()
    {
        if (munitionText != null)
        {
            TMP_Text textMeshPro = munitionText.GetComponent<TMP_Text>();
            if (textMeshPro != null)
            {
                textMeshPro.text = munition.ToString();
            }
        }
    }

    [PunRPC]

    void shoot()
    {
        
        if (Input.GetButtonDown("Fire1"))
        {
            if (munition > 0 && !isBurned)
            {
                photonView.RPC("UpdateShoot", RpcTarget.All);
                munition = 0;
                photonView.RPC("SetMunition", RpcTarget.All, munition);
                anim.SetTrigger("fire");
                photonView.RPC("UpdateAnimationTrigger", RpcTarget.Others, "fire");
            }
        }
    }

    [PunRPC]
    public void UpdateShoot()
    {
        GameObject temp = Instantiate(bullet);
        temp.transform.position = gun.position;
        direction = (transform.eulerAngles.y == 180) ? 1 : -1;
        temp.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shotForce * direction, 0f);
        temp.GetComponent<Bullet>().damage = munition;
        temp.GetComponent<Bullet>().shooter = playerNum;

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

        Destroy(temp.gameObject, 3f);
    }
    
    void Burned()
    {
        if (munition >= 4)
        {
            isBurned = true;
        }
    }

    void burn()
    {
        if (isBurned)
        {
            munition = 2;
            photonView.RPC("SetMunition", RpcTarget.All, munition);
            anim.SetTrigger("burn");
            photonView.RPC("UpdateAnimationTrigger", RpcTarget.Others, "burn");
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
        if (collision == null || collision.gameObject == null)
        {
            Debug.LogError("Collision ou collision.gameObject está nulo.");
            return;
        }

        if (collision.gameObject.layer == 8)
        {
            isJumping = false;

            if (anim != null)
            {
                anim.SetBool("jump", false);
                photonView.RPC("UpdateAnimationState", RpcTarget.Others, "jump", anim.GetBool("jump"));
            }
            else
            {
                Debug.LogWarning("Animator não foi inicializado.");
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.tag == "Pepper")
        {
            this.munition++;
            photonView.RPC("SetMunition", RpcTarget.All, munition);
        }

        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if(bullet != null && bullet.shooter != this.playerNum)
        {
            anim.SetTrigger("damage");
            photonView.RPC("UpdateAnimationTrigger", RpcTarget.Others, "damage");
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
        photonView.RPC("UpdateDust", RpcTarget.All, true);
    }

    void DisableDust()
    {
        dust.Stop();
        photonView.RPC("UpdateDust", RpcTarget.All, false);
    }

    void overCharge()
    {
        if (munition == 3 && !fire.isPlaying)
        {
            fire.Play();
            photonView.RPC("UpdateFire", RpcTarget.All, true);
        }
    }

}
