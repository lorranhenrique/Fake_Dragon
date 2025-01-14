using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TelaInicial : MonoBehaviour
{
    [SerializeField] private Button entrar;
    [SerializeField] private Button sair;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EntrarNoJogo();
        }
    }

    public void EntrarNoJogo()
    {
        SceneManager.LoadScene("Menu");
    }

    public void saiDoJogo()
    {
        Application.Quit();
    }
}
