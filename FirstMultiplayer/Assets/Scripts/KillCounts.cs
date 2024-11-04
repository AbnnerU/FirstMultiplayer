using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

using Random = UnityEngine.Random;

public class KillCounts : MonoBehaviour
{
    [SerializeField] private List<Kills> highestKills = new List<Kills>();
    [SerializeField] private KillAndNamesTexts[] textsElements;
    [SerializeField] private GameObject killCountPanel;
    [SerializeField] private PlayerNamesHud namesHud;
    [Header("Kill Message")]
    [SerializeField] private GameObject killMessageContainer;
    [SerializeField] private TMP_Text killerNameText;
    [SerializeField] private TMP_Text victimNameText;

    private InputController inputController;
    private PhotonView photonView;
    private bool tabOn = false;

    // Start is called before the first frame update
    void Awake()
    {
        killMessageContainer.SetActive(false);

        photonView = GetComponent<PhotonView>();    

        inputController = FindFirstObjectByType<InputController>();

        if (inputController)
        {
            inputController.OnKillCountTabInputPressed += OnKillCountTabInputPressed;
        }

        tabOn = false;
        killCountPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (inputController)
        {
            inputController.OnKillCountTabInputPressed -= OnKillCountTabInputPressed;
        }
    }

    public void ShowKillMessage(int hittedViewId, int shooterViewId)
    {
        Debug.Log("Show Message Request");
        photonView.RPC("KillMessage", RpcTarget.All, hittedViewId, shooterViewId);
    }


    [PunRPC]
    void KillMessage(int hittedViewId, int shooterViewId)
    {
        StopAllCoroutines();


        GameObject[] allPlayer = GameObject.FindGameObjectsWithTag("Player");
        string killerName = "";
        string victimName = "";

        for (int i = 0; i < allPlayer.Length; i++)
        {
            if (allPlayer[i].GetComponent<PhotonView>().ViewID == hittedViewId)
            {
                victimName = allPlayer[i].GetComponent<PhotonView>().Owner.NickName;
                break;
            }
        }

        if (victimName == string.Empty || victimName == "") return;

        for (int i = 0; i < allPlayer.Length; i++)
        {
            if (allPlayer[i].GetComponent<PhotonView>().ViewID == shooterViewId)
            {
                killerName = allPlayer[i].GetComponent<PhotonView>().Owner.NickName;
                break;
            }
        }

        if (killerName == string.Empty || killerName == "") return;

        killMessageContainer.SetActive(true);

        killerNameText.text = killerName;
        victimNameText.text = victimName;

        StartCoroutine(DisableKillMessageDelay());
    }

    IEnumerator DisableKillMessageDelay()
    {
        yield return new WaitForSeconds(3);
        photonView.RPC("DisableKillMessage", RpcTarget.All);
        yield break;
    }

    [PunRPC]
    void DisableKillMessage()
    {
        killMessageContainer.SetActive(false);
    }

    private void OnKillCountTabInputPressed()
    {
        if (tabOn)
        {
            tabOn = false;
            killCountPanel.SetActive(false);
        }
        else
        {
            tabOn = true;
            killCountPanel.SetActive(true);
            DisableAllKillCounts();

            string[] allPlayers = namesHud.GetAllConnectedPlayersNames();
            highestKills.Clear();

            for (int i = 0; i < allPlayers.Length; i++)
            {
                highestKills.Add(new Kills(allPlayers[i], Random.Range(1,100)));
            }

            highestKills.Sort();

            for(int i =0; i < highestKills.Count; i++)
            {
                textsElements[i].textElementsContainer.SetActive(true);
                textsElements[i].nameText.text = highestKills[i].playerName;
                textsElements[i].killCountText.text = highestKills[i].playerKills.ToString();
            }
        }
    }

    private void DisableAllKillCounts() { 
        for(int i=0; i < textsElements.Length; i++)
            textsElements[i].textElementsContainer.SetActive(false);    
    }

    [Serializable]
    private class Kills : IComparable<Kills>
    {
        public string playerName;
        public int playerKills;

        public Kills(string playerName, int playerKills)
        {
            this.playerName = playerName;
            this.playerKills = playerKills;
        }

        public int CompareTo(Kills other)
        {
            return other.playerKills - playerKills;
        }
    }

    [Serializable]
    public struct KillAndNamesTexts
    {
        public GameObject textElementsContainer;
        public TMP_Text nameText;
        public TMP_Text killCountText;
    }
}
