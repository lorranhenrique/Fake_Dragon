using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    

    private int jogadoresEmJogo = 0;

    [SerializeField] private string localizacaoPrefab;
    [SerializeField] private Transform[] spawns;
    private List<PlayerNet> jogadores;
    public List<PlayerNet> Jogadores { get => jogadores; private set => jogadores = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializar a lista de jogadores
        jogadores = new List<PlayerNet>();
    }


    private void Start()
    {
        photonView.RPC("AdicionaJogador", RpcTarget.All);
        jogadores = new List<PlayerNet>();
    }

    [PunRPC]
    private void AdicionaJogador()
    {
        jogadoresEmJogo++;

        if (PhotonNetwork.IsMasterClient && jogadoresEmJogo == PhotonNetwork.PlayerList.Length)
        {
            // Apenas o cliente mestre gerencia a criação global
            photonView.RPC("CriaJogadorRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void CriaJogadorRPC()
    {
        if (!PhotonNetwork.IsConnected || !photonView.IsMine)
            return;

        CriaJogador();
    }

    private void CriaJogador()
    {
        if (!PhotonNetwork.IsConnected || !photonView.IsMine)
            return;

        var spawnIndex = Random.Range(0, spawns.Length);
        var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[spawnIndex].position, Quaternion.identity);
        var jogador = jogadorOBJ.GetComponent<PlayerNet>();

        if (jogador != null)
        {
            jogador.photonView.RPC("Inicialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
    }


}

