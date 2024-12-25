using TMPro;
using UnityEngine;

public class UIMunition1 : MonoBehaviour
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
    }
}
