using UnityEngine;
using UnityEngine.UI;

public class MenuEntrada : MonoBehaviour
{
    [SerializeField] private Text nomeDoJogador;
    [SerializeField] private Text nomeDaSala;

    public void CriaSala()
    {
        if(!string.IsNullOrWhiteSpace(nomeDoJogador.text) && !string.IsNullOrWhiteSpace(nomeDaSala.text))
        {
            GestorDeRede.Instance.MudaNick(nomeDoJogador.text);
            GestorDeRede.Instance.CriaSala(nomeDaSala.text);
            return;
        }
        Debug.LogWarning("O nome do jogador ou o nome da sala está vazio!");

    }
    public void EntraSala()
    {
        if (!string.IsNullOrWhiteSpace(nomeDoJogador.text) && !string.IsNullOrWhiteSpace(nomeDaSala.text))
        {
            GestorDeRede.Instance.MudaNick(nomeDoJogador.text);
            GestorDeRede.Instance.EntraSala(nomeDaSala.text);
            return;
        }
        Debug.LogWarning("O nome do jogador ou o nome da sala está vazio!");
    }
}
