using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Button exit;
    [SerializeField] private Button restart;

 
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    [PunRPC]
    public void ReturnMenu()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("TelaInicial");
    }


    public void Restart()
    {
        PhotonNetwork.LoadLevel("Scene 1");
    }

}
