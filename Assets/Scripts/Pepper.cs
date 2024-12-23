using System;
using UnityEngine;

public class Pepper : MonoBehaviour
{

    private SpriteRenderer sr;
    private BoxCollider2D box;
    public GameObject collected;
    public bool coletada;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player" && !coletada)
        {
            coletada = true;
            AdicionarMunicao();
        }
    }

    void AdicionarMunicao()
    {

        Player.Instance.munition++;

        sr.enabled = false;
        box.enabled = false;
        collected.SetActive(true);



        if (Player.Instance.munition == 4)
        {
            Player.Instance.isBurned = true;
        }

        Destroy(gameObject, 0.6f);
    }

    public event Action OnDestroyed;

    private void OnDestroy()
    {
        
        OnDestroyed?.Invoke();
    }
}

