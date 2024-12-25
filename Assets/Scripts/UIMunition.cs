using TMPro;
using UnityEngine;

public class UIMunition : MonoBehaviour
{
    public TMP_Text textMeshPro;

    void Awake()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TMP_Text>();
        }
    }

    void Update()
    {
        if (Player.Instance != null && textMeshPro != null)
        {
            textMeshPro.text = Player.Instance.munition.ToString();
        }
        if (Player2.Instance != null && textMeshPro != null)
        {
            textMeshPro.text = Player2.Instance.munition.ToString();
        }
    }
}
