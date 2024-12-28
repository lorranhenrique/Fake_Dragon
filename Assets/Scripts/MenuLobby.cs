using UnityEngine;
using UnityEngine.UI;

public class MenuLobby : MonoBehaviour
{
    [SerializeField] private Text listaDeJogadores;
    [SerializeField] private Button comecaJogo;

    public void AtualizaLista()
    {
        listaDeJogadores.text = GestorDeRede.Instance.ObterListaDeJogadores();
        comecaJogo.interactable = GestorDeRede.Instance.DonoDaSala();
    }
}
