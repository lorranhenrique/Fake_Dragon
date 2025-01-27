using Photon.Pun;
using UnityEngine;

public class GestorDeRede : MonoBehaviourPunCallbacks
{
    public static GestorDeRede Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        Debug.Log("Tentando conectar ao Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conex�o com o servidor Photon bem-sucedida!");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Falha na conex�o: {cause}");
    }

    public void CriaSala(string nomeSala)
    {
        PhotonNetwork.CreateRoom(nomeSala);
    }

    public void EntraSala(string nomeSala)
    {
        PhotonNetwork.JoinRoom(nomeSala);
    }

    public void MudaNick(string nickName)
    {
        PhotonNetwork.NickName = nickName;
        
    }

    public string ObterListaDeJogadores()
    {
        var lista = "";
        foreach(var player in PhotonNetwork.PlayerList)
        {
            lista += player.NickName + "\n";
        }

        return lista;
    }

    public bool DonoDaSala()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public void SairDoLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void ComecaJogo( string nomeCena)
    {
        PhotonNetwork.LoadLevel("Scene 1");
    }
}
