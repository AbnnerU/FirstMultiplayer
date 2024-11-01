using Photon.Pun;

using UnityEngine;
using TMPro;

public class PlayerCheck : MonoBehaviour
{
    [SerializeField] private  int maxPlayersInRoom = 2;
    [SerializeField] private TMP_Text currentPlayers;
    [SerializeField] private GameObject panel;

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersInRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            this.panel.SetActive(false);
        }
        currentPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString
        () + "/" + maxPlayersInRoom.ToString();
    }
}
