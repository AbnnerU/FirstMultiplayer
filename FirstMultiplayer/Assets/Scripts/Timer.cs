using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text minutesText;
    [SerializeField] private TMP_Text secondsText;

    [SerializeField] private PhotonView photonView;

    [SerializeField]private int minutes = 4;
    [SerializeField]private int seconds = 59;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        
    }

    public void BeginTimer()
    {
        photonView.RPC("Count", RpcTarget.AllBuffered);
        
    }

    [PunRPC]
    void Count()
    {
        BeginCounting();
        minutesText.text = minutes.ToString();
        secondsText.text = seconds.ToString();
    }
    void BeginCounting()
    {
        CancelInvoke();
        InvokeRepeating("TimeCountDown", 1, 1);
    }
    void TimeCountDown()
    {    
        if (seconds > 10)
        {
            seconds -= 1;
            secondsText.text = seconds.ToString();
        }
        else if (seconds > 0 && seconds < 11)
        {
            seconds -= 1;
            secondsText.text = "0" + seconds.ToString();
        }
        else if (seconds == 0 && minutes > 0)
        {
            secondsText.text = "0" + seconds.ToString();
            minutes -= 1;
            seconds = 59;
            minutesText.text = minutes.ToString();
            secondsText.text = seconds.ToString();
        }
    }
}
