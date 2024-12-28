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
        menuLobby.AtualizaLista();
    }

    public void mudaMenu(GameObject menu)
    {
        menuEntrada.gameObject.SetActive(false);
        menuLobby.gameObject.SetActive(false);

        menu.SetActive(true);
    }
}
