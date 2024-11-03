using Photon.Pun;

using UnityEngine;
using TMPro;

public class PlayerCheck : MonoBehaviour
{
    [SerializeField] private  int maxPlayersInRoom = 2;
    [SerializeField] private TMP_Text currentPlayers;
    [SerializeField] private TMP_Text[] labels;
    [SerializeField] private GameObject enterButton;
    [SerializeField] private GameObject panel;
    private bool playerEnter = false;
    private void Awake()
    {
        enterButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerEnter) return;
        //if(PhotonNetwork.CurrentLobby==null) return;
       
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersInRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            for (int i = 0; i < labels.Length; i++)
                labels[i].enabled = false;

            enterButton.SetActive(true);

            //this.panel.SetActive(false);
        }
        else
        {
            for (int i = 0; i < labels.Length; i++)
                labels[i].enabled = true;

            enterButton.SetActive(false);

            currentPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString
            () + "/" + maxPlayersInRoom.ToString();
        }
    }

    private void OnDestroy()
    {
        playerEnter = false;
    }

    public bool IsWaitingScreenActive()
    {
        return panel.activeSelf;
    }

    public void Enter()
    {
        panel.SetActive(false);
        playerEnter = true;
    }
}
