using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Button exit;
    [SerializeField] private Button restart;

    public string namo = "Scene 1";
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    [PunRPC]
    public void ReturnMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("TelaInicial");
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
