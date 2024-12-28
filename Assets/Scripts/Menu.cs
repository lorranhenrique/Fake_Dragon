using Photon.Pun;
using UnityEngine;

public class Menu : MonoBehaviourPunCallbacks
{
    [SerializeField] private MenuEntrada menuEntrada;
    [SerializeField] private MenuLobby menuLobby;

    private void Start()
    {
        menuEntrada.gameObject.SetActive(false);
        menuLobby.gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        menuEntrada.gameObject.SetActive(true);
        //menuLobby.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        mudaMenu(menuLobby.gameObject);
        menuLobby.photonView.RPC("AtualizaLista", RpcTarget.All);
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
        mudaMenu(menuEntrada.gameObject);
    }

    public void ComecaJogo(string nomeCena)
    {
        GestorDeRede.Instance.photonView.RPC("ComecaJogo", RpcTarget.All, nomeCena);
        mudaMenu(menuEntrada.gameObject);
    }
}
