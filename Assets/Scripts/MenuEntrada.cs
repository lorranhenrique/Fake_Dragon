using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MenuEntrada : MonoBehaviour
{
    [SerializeField] private Text nomeDoJogador;
    [SerializeField] private Text nomeDaSala;
    [SerializeField] private GameObject alert;

    private bool ValidarTexto(string texto)
    {
        return Regex.IsMatch(texto, @"^[a-zA-Z\s]+$");
    }

    public void CriaSala()
    {
        if (string.IsNullOrWhiteSpace(nomeDoJogador.text) || string.IsNullOrWhiteSpace(nomeDaSala.text))
        {
            alert.gameObject.SetActive(true);
            return;
        }

        if (!ValidarTexto(nomeDoJogador.text) || !ValidarTexto(nomeDaSala.text))
        {
            alert.gameObject.SetActive(true);
            return;
        }

        alert.gameObject.SetActive(false);
        GestorDeRede.Instance.MudaNick(nomeDoJogador.text);
        GestorDeRede.Instance.CriaSala(nomeDaSala.text);
    }

    public void EntraSala()
    {
        if (string.IsNullOrWhiteSpace(nomeDoJogador.text) || string.IsNullOrWhiteSpace(nomeDaSala.text))
        {
            alert.gameObject.SetActive(true);
            return;
        }

        if (!ValidarTexto(nomeDoJogador.text) || !ValidarTexto(nomeDaSala.text))
        {
            alert.gameObject.SetActive(true);
            return;
        }

        alert.gameObject.SetActive(false);
        GestorDeRede.Instance.MudaNick(nomeDoJogador.text);
        GestorDeRede.Instance.EntraSala(nomeDaSala.text);
    }
}
