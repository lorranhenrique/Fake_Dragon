using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
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
    [SerializeField] public GameObject placarEmpate;
    [SerializeField] public GameObject Botoes;
    [SerializeField] public GameObject telaDePause;


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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OpenPause();
        }
    }

    public void OpenPause()
    {
        if (!telaDePause.gameObject.activeSelf)
        {
            telaDePause.gameObject.SetActive(true);
            photonView.RPC("AtualizaPausaPlayer", RpcTarget.All, true);
            return;
        }
        ClosePause();
    }

    public void ClosePause()
    {
        telaDePause.gameObject.SetActive(false);
        photonView.RPC("AtualizaPausaPlayer", RpcTarget.All,false);
    }

    [PunRPC]

    void AtualizaPausaPlayer(bool situação)
    {
        PlayerNet.Instance.pausado = situação;
    }

    public void ReturnMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("TelaInicial");
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

    public void VerificaFimDeJogo()
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
                if (player.life > 0 && !GameManager.Instance.placarVitoria.activeSelf)
                {
                    player.photonView.RPC("UpdateVitoria", RpcTarget.All);
                }
                else if (player.life <= 0 && !GameManager.Instance.placarDerrota.activeSelf)
                {
                    player.photonView.RPC("UpdateDerrota", RpcTarget.All);
                }
            }
        }
        else if (jogadoresVivos == 0)
        {
            StartCoroutine(CheckEmpateDelayed());
        }
    }


    private IEnumerator CheckEmpateDelayed()
    {
        yield return new WaitForSeconds(0.6f);
        UpdateEmpate();
    }

    [PunRPC]
    public void UpdateEmpate()
    {
        if (Instance != null)
        {
            Instance.placarVitoria.SetActive(false);
            Instance.placarDerrota.SetActive(false);
            Instance.placarEmpate.SetActive(true);
            Instance.Botoes.SetActive(true);
        }
    }

    [PunRPC]
    public void UpdateVitoria()
    {
        if (GameManager.Instance != null && !GameManager.Instance.placarEmpate.activeSelf)
        {
            GameManager.Instance.placarDerrota.SetActive(false);
            GameManager.Instance.placarVitoria.SetActive(true);
            GameManager.Instance.Botoes.SetActive(true);
        }
    }

    [PunRPC]
    public void UpdateDerrota()
    {
        if (GameManager.Instance != null && !GameManager.Instance.placarEmpate.activeSelf)
        {
            GameManager.Instance.placarVitoria.SetActive(false);
            GameManager.Instance.placarDerrota.SetActive(true);
            GameManager.Instance.Botoes.SetActive(true);
        }
    }


}
