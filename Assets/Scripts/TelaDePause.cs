using UnityEngine;
using UnityEngine.UI;

public class TelaDePause : MonoBehaviour
{

    [SerializeField] private Button continuar;
    [SerializeField] private Button options;
    [SerializeField] private Button quit;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void SetContinueButtonInteractable(bool isInteractable)
    {
        continuar.interactable = isInteractable;
    }
}
