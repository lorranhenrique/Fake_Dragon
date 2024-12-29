using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    private int jogadoresEmJogo = 0;

    [SerializeField] private string localizacaoPrefab;
    [SerializeField] private Transform[] spawns;
    private bool[] spawnsOcupados;
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

        spawnsOcupados = new bool[spawns.Length];
    }

    private void Start()
    {
        photonView.RPC("AdicionaJogador", RpcTarget.AllBuffered);
        jogadores = new List<PlayerNet>();
    }

    [PunRPC]
    private void AdicionaJogador()
    {
        jogadoresEmJogo++;

        if (jogadoresEmJogo == PhotonNetwork.PlayerList.Length)
        {
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
            spawnIndex = Random.Range(0, spawns.Length);
        }
        else
        {
            spawnIndex = System.Array.FindIndex(spawnsOcupados, ocupado => !ocupado);
        }

        spawnsOcupados[spawnIndex] = true;

        var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[spawnIndex].position, Quaternion.identity);
        var jogador = jogadorOBJ.GetComponent<PlayerNet>();

       
        jogador.photonView.RPC("Inicialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
          
    }
}
