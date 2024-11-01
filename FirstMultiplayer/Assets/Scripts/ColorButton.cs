using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorButton : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private GameObject[] players;
    private int id;
    private Timer timer;

    private void Start()
    {
        Cursor.visible = true;

        panel = GameObject.FindGameObjectWithTag("ColorChoosePanel");
        timer = FindFirstObjectByType<Timer>();
    }

    public void SelectButton(int buttonNumber)
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PhotonView>().IsMine == true)
            {
                id = players[i].GetComponent<PhotonView>().ViewID;
                break;
            }
        }

        GetComponent<PhotonView>().RPC("SelectedColor", RpcTarget.AllBuffered, buttonNumber, id);

        panel.SetActive(false);
        Cursor.visible = false;
    }

    [PunRPC]
    void SelectedColor(int buttonNumber, int phothonViewId)
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log("Color button: Selected color "+buttonNumber);

            players[i].GetComponent<DisplayColor>().ChooseColor(phothonViewId, buttonNumber);

            
            break;
        }

        timer.BeginTimer();

        this.gameObject.SetActive(false);
    }
}
