using UnityEngine;

public class Pepper : MonoBehaviour
{

    private SpriteRenderer sr;
    private BoxCollider2D box;
    public GameObject collected;


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
        if(collider.gameObject.tag == "Player")
        {
            sr.enabled = false;
            box.enabled = false;
            collected.SetActive(true);

            Player.Instance.munition++;

            Destroy(gameObject,0.6f);
        }
    }
}
