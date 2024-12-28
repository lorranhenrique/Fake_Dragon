using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    private int jogadoresEmJogo = 0;

    [SerializeField] private string localizacaoPrefab;
    [SerializeField] private Transform[] spawns;
    private bool[] spawnsOcupados; // Controle de spawns ocupados
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

        // Inicializar a lista de jogadores e controle de spawns
        jogadores = new List<PlayerNet>();
        spawnsOcupados = new bool[spawns.Length];
    }

    private void Start()
    {
        photonView.RPC("AdicionaJogador", RpcTarget.All);
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

        int spawnIndex;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // Primeiro jogador: spawn aleatório
            spawnIndex = Random.Range(0, spawns.Length);
        }
        else
        {
            // Segundo jogador: ocupa o spawn vazio
            spawnIndex = System.Array.FindIndex(spawnsOcupados, ocupado => !ocupado);
        }

        // Marca o spawn como ocupado
        spawnsOcupados[spawnIndex] = true;

        // Instancia o jogador no spawn selecionado
        var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[spawnIndex].position, Quaternion.identity);
        var jogador = jogadorOBJ.GetComponent<PlayerNet>();

        if (jogador != null)
        {
            jogador.photonView.RPC("Inicialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
            jogadores.Add(jogador); // Adiciona o jogador à lista
        }
    }
}
