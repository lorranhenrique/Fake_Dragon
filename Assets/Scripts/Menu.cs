using Photon.Pun;
using UnityEngine;
public class Menu : MonoBehaviourPunCallbacks
{
    [SerializeField] private MenuEntrada menuEntrada;
    [SerializeField] private MenuLobby menuLobby;
    [SerializeField] private TelaDeCarregamento telaDeCarregamento;

    private void Start()
    {
        menuEntrada.gameObject.SetActive(false);
        menuLobby.gameObject.SetActive(false);
        telaDeCarregamento.gameObject.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {   
        if(menuLobby != null)
        { 
            telaDeCarregamento.gameObject.SetActive(false);
            menuEntrada.gameObject.SetActive(true);
        }
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
        menuLobby.gameObject.SetActive(false);
        menuEntrada.gameObject.SetActive(true);
    }

    public void ComecaJogo(string nomeCena)
    {
        GestorDeRede.Instance.photonView.RPC("ComecaJogo", RpcTarget.All, nomeCena);
    }
}
