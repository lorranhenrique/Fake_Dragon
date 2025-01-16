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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player" && !coletada)
        {
            coletada = true;
            sr.enabled = false;
            box.enabled = false;
            collected.SetActive(true);

            Destroy(gameObject, 0.6f);
        }

    }

    public event Action OnDestroyed;

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}

