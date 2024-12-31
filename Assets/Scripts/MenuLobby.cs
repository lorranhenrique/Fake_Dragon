using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text listaDeJogadores;
    [SerializeField] private Button comecaJogo;

    [PunRPC] 

    public void AtualizaLista()
    {
        listaDeJogadores.text = GestorDeRede.Instance.ObterListaDeJogadores();
        comecaJogo.interactable = GestorDeRede.Instance.DonoDaSala();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        AtualizaLista();
    }


}
