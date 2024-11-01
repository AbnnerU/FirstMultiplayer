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

    private string playerName = "";
    private int playerPhothonViewId = 0;
    private Color colorRef;

    private void Awake()
    {
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
