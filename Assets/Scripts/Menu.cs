using System.Collections;
using Photon.Pun;
using UnityEngine;
public class Menu : MonoBehaviourPunCallbacks
{
    [SerializeField] private MenuEntrada menuEntrada;
    [SerializeField] private MenuLobby menuLobby;
    [SerializeField] private TelaDeCarregamento telaDeCarregamento;
    [SerializeField] private TelaDePause telaDePause;
    [SerializeField] private GameObject pressSpace;

    private void Start()
    {
        menuEntrada.gameObject.SetActive(false);
        menuLobby.gameObject.SetActive(false);
        telaDeCarregamento.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { 
            OpenPause();
        }

        if (pressSpace.activeSelf && menuLobby != null && Input.GetKeyDown(KeyCode.Space))
        {
            telaDeCarregamento.gameObject.SetActive(false);
            menuEntrada.gameObject.SetActive(true);
            pressSpace.SetActive(false);
        }
    }

    public void OpenPause()
    {
        if (!telaDePause.gameObject.activeSelf)
        {
            telaDePause.gameObject.SetActive(true);
            return;
        }
        ClosePause();
    }

    public void ClosePause()
    {
        telaDePause.gameObject.SetActive(false);
    }

    public void ReturnMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("TelaInicial");
    }

    public override void OnConnectedToMaster()
    {
        pressSpace.SetActive(true);
       
    }

    public override void OnJoinedRoom()
    {
        mudaMenu(menuLobby.gameObject);
        menuLobby.photonView.RPC("AtualizaLista", RpcTarget.AllBuffered);
    }

    public void mudaMenu(GameObject menu)
    {
        menuEntrada.gameObject.SetActive(false);
        menuLobby.gameObject.SetActive(false);
        menu.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        menuLobby.AtualizaLista();
    }

    public void SairDoLobby()
    {
        GestorDeRede.Instance.SairDoLobby();
        PhotonNetwork.LoadLevel("Menu");
    }

    public void ComecaJogo(string nomeCena)
    {
        GestorDeRede.Instance.photonView.RPC("ComecaJogo", RpcTarget.All, nomeCena);
    }
}
