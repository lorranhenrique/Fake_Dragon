using UnityEngine;
using UnityEngine.UI;

public class MenuEntrada : MonoBehaviour
{
    [SerializeField] private Text nomeDoJogador;
    [SerializeField] private Text nomeDaSala;

    public void CriaSala()
    {
        GestorDeRede.Instance.MudaNick(nomeDoJogador.text);
        GestorDeRede.Instance.CriaSala(nomeDaSala.text);
    }
    public void EntraSala()
    {
        GestorDeRede.Instance.MudaNick(nomeDoJogador.text);
        GestorDeRede.Instance.EntraSala(nomeDaSala.text);
    }
}
