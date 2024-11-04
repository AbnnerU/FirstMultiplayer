using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNamesHud : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject statsPrefab;
    [SerializeField] private PhotonView photonViewRef;
    [SerializeField] private Transform parent;


    private List<PlayerHudStats> playersStatsList = new List<PlayerHudStats>();

    private void Awake()
    {
        photonViewRef = GetComponent<PhotonView>();
    }

    public void SetNewPlayer(string nickName, Color color, int playerPhothonViewId)
    {
        Debug.Log("PlayersNameHud: request new (" + playerPhothonViewId + ")");
        string stringColor = ColorUtility.ToHtmlStringRGB(color);

        photonViewRef.RPC("AddPlayerStatsOnHud", RpcTarget.AllBuffered, nickName, stringColor, playerPhothonViewId);
    }

    public void RemovePlayer(int playerPhothonViewId)
    {
        Debug.Log("PlayersNameHud: request remove (" + playerPhothonViewId + ")");

        photonViewRef.RPC("RemovePlayerStatsOnHud", RpcTarget.All, playerPhothonViewId);
    }

    public void Leaving()
    {
        StartCoroutine(BackToLobby());
    }

    IEnumerator BackToLobby()
    {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void RoomExit()
    {
        StartCoroutine(ToLobby());
    }

    IEnumerator ToLobby()
    {
        yield return new WaitForSeconds(0.5f);
        Cursor.visible = true;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
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


    public void TryUpdatePlayerHealthStats(int hittedViewId, int shooterViewId, float damage)
    {
        photonViewRef.RPC("DoDamageToPlayer", RpcTarget.All, hittedViewId, shooterViewId, damage);
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

    [PunRPC]
    public void RemovePlayerStatsOnHud( int playerPhothonViewId)
    {
        for (int i = 0; i < playersStatsList.Count; i++)
        {
            if (playersStatsList[i].GetPlayerViewId() == playerPhothonViewId)
            {
                Debug.Log("ID (" + playerPhothonViewId + ") removed");

                PhotonNetwork.Destroy(playersStatsList[i].gameObject);

                playersStatsList.RemoveAt(i);
                return;
            }
        }

        Debug.Log("CANT FIND " + playerPhothonViewId + " TO REMOVE");
    }

    [PunRPC]
    public void DoDamageToPlayer(int hittedViewId, int shooterViewId, float damage)
    {
        for (int i = 0; i < playersStatsList.Count; i++)
        {
            if (playersStatsList[i].GetPlayerViewId() == hittedViewId)
            {
                Debug.Log("(" + shooterViewId + ") hitted (" + hittedViewId + "). Damage: "+damage);
                playersStatsList[i].UpdateBar(hittedViewId, shooterViewId, damage);
                return;
            }
        }
    }
}
