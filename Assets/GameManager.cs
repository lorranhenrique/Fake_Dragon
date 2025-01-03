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
        int jogadorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; 

        if (jogadorIndex >= 0 && jogadorIndex < spawns.Length)
        {
            if (!spawnsOcupados[jogadorIndex])
            {
                spawnsOcupados[jogadorIndex] = true;

                var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[jogadorIndex].position, Quaternion.identity);
                var jogador = jogadorOBJ.GetComponent<PlayerNet>();

                jogador.photonView.RPC("Inicialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);

                Debug.Log($"Jogador {PhotonNetwork.LocalPlayer.NickName} spawnado na posição {jogadorIndex}.");
            }
            else
            {
                Debug.LogWarning($"O spawn {jogadorIndex} já está ocupado!");
            }
        }
        else
        {
            Debug.LogError($"Índice de spawn inválido: {jogadorIndex}");
        }
    }
}
