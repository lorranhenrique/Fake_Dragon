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
            DontDestroyOnLoad(gameObject);
            spawnsOcupados = new bool[spawns.Length];
        }
        else
        {
            Destroy(gameObject);
        }
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

        if (PhotonNetwork.IsMasterClient && jogadoresEmJogo == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("CriaJogador", RpcTarget.AllBuffered);
            Debug.Log("CHAMOU O CRIA JOGADOR: "+jogadoresEmJogo);

        }
        Debug.Log(jogadoresEmJogo);
    }

    [PunRPC]
private void CriaJogador()
{
    if (!PhotonNetwork.IsMasterClient) return;

    for (int i = 0; i < spawns.Length; i++)
    {
        if (!spawnsOcupados[i])
        {
            spawnsOcupados[i] = true;
            photonView.RPC("SpawnJogador", RpcTarget.AllBuffered, i);
            return;
        }
    }

    Debug.LogWarning("Todos os pontos de spawn estão ocupados!");
}



    [PunRPC]
    private void SpawnJogador(int spawnIndex)
    {
        if (spawnIndex >= 0 && spawnIndex < spawns.Length)
        {
            var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[spawnIndex].position, Quaternion.identity);

            var jogador = jogadorOBJ.GetComponent<PlayerNet>();
            jogador.photonView.RPC("Inicialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        else
        {
            Debug.LogError("Índice de spawn inválido recebido: " + spawnIndex);
        }
    }


}
