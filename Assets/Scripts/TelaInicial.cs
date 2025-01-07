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
        
    }

    public void EntrarNoJogo()
    {
        SceneManager.LoadScene("Menu");
    }
}
