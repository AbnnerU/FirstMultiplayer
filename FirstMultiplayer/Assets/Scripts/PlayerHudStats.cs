using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHudStats : MonoBehaviour
{
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private KillCounts killCounts;
    [Range(0,100)]
    [SerializeField] private float healthValue=100;

    private string playerName = "";
    private int playerPhothonViewId = 0;
    private bool isAlive = true;
    private Color colorRef;

    private void Awake()
    {
        killCounts = FindFirstObjectByType<KillCounts>();
        photonView = GetComponent<PhotonView>();
    }

    public void SetStatsInfo(string nickName, string color, int playerPhothonViewId)
    {
        photonView.RPC("ApplyStats", RpcTarget.AllBuffered, nickName, color, playerPhothonViewId);
    }

    public int GetPlayerViewId()
    {
        return playerPhothonViewId;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void UpdateBar(int hittedViewId, int shooterViewId, float damage)
    {
        photonView.RPC("HealthUpdate", RpcTarget.AllBuffered, hittedViewId, shooterViewId, damage);
    }

    [PunRPC]
    public void HealthUpdate(int hittedViewId, int shooterViewId, float damage)
    {
        if(hittedViewId == this.playerPhothonViewId)
        {
            if (!isAlive) return;

            this.healthValue -= damage;

            float percentage = this.healthValue / 100;

            this.healthBarImage.fillAmount = percentage;

            GameObject[] allPlayer = GameObject.FindGameObjectsWithTag("Player");

            for(int i = 0; i < allPlayer.Length; i++)
            {
                if (allPlayer[i].GetComponent<PhotonView>().ViewID == hittedViewId)
                {
                    allPlayer[i].GetComponent<HealthHandler>().DoDamage((int)this.healthValue, hittedViewId);
                   
                    if (this.healthValue <= 0)
                    {
                        allPlayer[i].GetComponent<HealthHandler>().ShowKillMessage(hittedViewId, shooterViewId);
                        this.healthValue = 0;
                        this.isAlive = false;                    
                    }

                    return;
                }
            }
        }
    }

    [PunRPC]
    public void ApplyStats(string nickName, string color, int playerPhothonViewId)
    {
        Color colorRef;
        ColorUtility.TryParseHtmlString("#"+color, out colorRef);

       // Debug.Log("Arrived color " + color);

        playerName = nickName;
        nickNameText.text = nickName;
        healthBarImage.color = colorRef;

        this.playerPhothonViewId = playerPhothonViewId;
        this.colorRef = colorRef;
    }

    
}
