using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TelaInicial : MonoBehaviour
{

    [SerializeField] private Button entrar;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            EntrarNoJogo();
        }
    }

    public void EntrarNoJogo()
    {
        SceneManager.LoadScene("Menu");
    }
}
