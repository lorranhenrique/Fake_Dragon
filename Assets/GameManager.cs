using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public int jogadoresEmJogo = 0;

    [SerializeField] private string localizacaoPrefab;
    [SerializeField] private Transform[] spawns;
    private bool[] spawnsOcupados;
    private List<PlayerNet> jogadores;
    public List<PlayerNet> Jogadores { get => jogadores; set => jogadores = value; }

    [SerializeField] public GameObject placarVitoria;
    [SerializeField] public GameObject placarDerrota;
    [SerializeField] public GameObject Botoes;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        spawnsOcupados = new bool[spawns.Length];
    }
    
    private void Start()
    {
        photonView.RPC("AdicionaJogador", RpcTarget.AllBuffered);
        jogadores = new List<PlayerNet>();
    }

    public void OnSceneLoaded(int sceneBuildIndex, string sceneName)
    {
        if (PhotonNetwork.PlayerList.Length == jogadoresEmJogo)
        {
            photonView.RPC("CriaJogador", RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    private void AdicionaJogador()
    {
        jogadoresEmJogo ++;

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
        int jogadorIndex = PhotonNetwork.LocalPlayer.ActorNumber -1;
        Debug.Log($"Jogador {PhotonNetwork.LocalPlayer.NickName} tem número {jogadorIndex}");

        if (jogadorIndex >= 0 && jogadorIndex < spawns.Length)
        {
            if (!spawnsOcupados[jogadorIndex])
            {
                photonView.RPC("UpdateSpawns", RpcTarget.AllBuffered, jogadorIndex);

                var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[jogadorIndex].position, Quaternion.identity);
                var jogador = jogadorOBJ.GetComponent<PlayerNet>();

                jogador.photonView.RPC("Inicialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, jogadorIndex);

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
            Invoke("FindSpawn", 0.3f);
        }
    }

    private void FindSpawn()
    {
        for (int i = 0; i < spawns.Length; i++)
        {
            if (!spawnsOcupados[i])
            {
                photonView.RPC("UpdateSpawns", RpcTarget.AllBuffered, i);

                var jogadorOBJ = PhotonNetwork.Instantiate(localizacaoPrefab, spawns[i].position, Quaternion.identity);
                var jogador = jogadorOBJ.GetComponent<PlayerNet>();

                jogador.photonView.RPC("Inicialize", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, i);

                Debug.Log($"Jogador {PhotonNetwork.LocalPlayer.NickName} spawnado na posição {i}.");
            }
        }
        Debug.LogWarning("Nenhum spawn vazio disponível!");
    }

    [PunRPC]
    private void UpdateSpawns(int i)
    {
        spawnsOcupados[i] = true;
    }


    [PunRPC]
    private void VerificaFimDeJogo()
    {
        int jogadoresVivos = 0;
        foreach (var jogador in jogadores)
        {
            if (jogador.life > 0)
            {
                jogadoresVivos++;
            }
        }

        if (jogadoresVivos == 1)
        {
            foreach (var player in jogadores)
            {
                if (player.life > 0)
                {
                    player.photonView.RPC("UpdateVitoria", RpcTarget.All);
                }
                else
                {
                    player.photonView.RPC("UpdateDerrota", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    public void UpdateVitoria()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.placarVitoria.SetActive(true);
            GameManager.Instance.Botoes.SetActive(true);
        }
    }

    [PunRPC]
    public void UpdateDerrota()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.placarDerrota.SetActive(true);
            GameManager.Instance.Botoes.SetActive(true);
        }
    }

}
