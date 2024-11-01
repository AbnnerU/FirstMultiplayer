using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNamesHud : MonoBehaviour
{
    [SerializeField] private GameObject statsPrefab;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Transform parent;


    private List<PlayerHudStats> playersStatsList = new List<PlayerHudStats>();

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SetNewPlayer(string nickName, Color color, int playerPhothonViewId)
    {
        Debug.Log("PlayersNameHud: request new (" + playerPhothonViewId + ")");
        string stringColor = ColorUtility.ToHtmlStringRGB(color);

        photonView.RPC("AddPlayerStatsOnHud", RpcTarget.AllBuffered, nickName, stringColor, playerPhothonViewId);
    }

    public string[] GetAllConnectedPlayersNames()
    {
        List<string> allNames = new List<string>();

        for(int i = 0; i < playersStatsList.Count; i++)
        {
            allNames.Add(playersStatsList[i].GetPlayerName());
        }

        return allNames.ToArray();
    }
    
    [PunRPC]
    public void AddPlayerStatsOnHud(string nickName, string color, int playerPhothonViewId)
    {
        for (int i = 0; i < playersStatsList.Count; i++)
        {
            if (playersStatsList[i].GetPlayerViewId() == playerPhothonViewId)
            {
                Debug.Log("ID ("+playerPhothonViewId+") already difined here");
                return;
            }
        }

        GameObject obj = PhotonNetwork.Instantiate(statsPrefab.name, Vector3.zero, Quaternion.identity);

        obj.transform.parent = parent;

        PlayerHudStats playerHudStats = obj.GetComponent<PlayerHudStats>();

        playerHudStats.SetStatsInfo(nickName, color, playerPhothonViewId);

        playersStatsList.Add(playerHudStats);

        Debug.Log("Player added to the hud");
        Debug.Log(playersStatsList.Count);
    }
    
}
