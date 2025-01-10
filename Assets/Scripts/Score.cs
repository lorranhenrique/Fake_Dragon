using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button exit;
    [SerializeField] private Button restart;

    void Start()
    {
        restart.interactable = GestorDeRede.Instance.DonoDaSala();
    }

    public void ReturnMenu()
    {
        photonView.RPC("UpdateRestartButton", RpcTarget.AllBuffered, true);
        StartCoroutine(LeaveRoomAfterDelay());
    }

    private IEnumerator LeaveRoomAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("TelaInicial");
    }

    [PunRPC]
    public void UpdateRestartButton(bool saiu)
    {
        if (saiu)
        {
            restart.interactable = false;
        }
    }

    public void RestartJogo()
    {
        photonView.RPC("ReiniciaCena", RpcTarget.AllBuffered);
        SceneManager.LoadScene("Scene 1");
    }

    [PunRPC]
    public void ReiniciaCena()
    {
        restart.interactable = true;
    }
}
