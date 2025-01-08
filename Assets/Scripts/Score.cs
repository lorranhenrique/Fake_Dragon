using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button exit;
    [SerializeField] private Button restart;

 
    void Start()
    {
        restart.interactable = GestorDeRede.Instance.DonoDaSala();
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


    public void RestartJogo()
    {
        photonView.RPC("ReiniciaCena", RpcTarget.AllBuffered);
        SceneManager.LoadScene("Scene 1");
    }

    [PunRPC]
    public void ReiniciaCena()
    {
        restart.interactable = true;
    }

}
