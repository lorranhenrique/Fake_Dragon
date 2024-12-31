using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    private int jogadoresEmJogo = 0;

    [SerializeField] private string localizacaoPrefab;
    [SerializeField] private Transform[] spawns;
    private bool[] spawnsOcupados;
    private List<PlayerNet> jogadores;
    public List<PlayerNet> Jogadores { get => jogadores; set => jogadores = value; }

    private void Awake()
    {
        //Jogadores = new List<PlayerNet>();
        spawnsOcupados = new bool[spawns.Length];
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém o GameManager ao trocar de cena
             // Inicializa a lista
        }
        else
        {
            Destroy(gameObject); // Garante que exista apenas um GameManager
        }
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

        if (jogadoresEmJogo == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("CriaJogador", RpcTarget.All);
            
        }
    }

    [PunRPC]

    private void CriaJogador()
    {
        int spawnIndex = System.Array.FindIndex(spawnsOcupados, ocupado => !ocupado);
        spawnsOcupados[spawnIndex] = true;

        var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[spawnIndex].position, Quaternion.identity);
        var jogador = jogadorOBJ.GetComponent<PlayerNet>();

        jogador.photonView.RPC("Inicialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }

}
