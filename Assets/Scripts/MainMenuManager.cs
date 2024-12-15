using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private string levelName;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void jogar()
    {
        SceneManager.LoadScene(levelName);
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}
