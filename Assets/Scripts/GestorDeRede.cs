using Photon.Pun;
using UnityEngine;

public class GestorDeRede : MonoBehaviourPunCallbacks
{
    public static GestorDeRede Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        Debug.Log("Tentando conectar ao Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexão com o servidor Photon bem-sucedida!");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Falha na conexão: {cause}");
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

    


}
