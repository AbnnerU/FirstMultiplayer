using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text roomNumberText;
    TypedLobby killCount = new TypedLobby("killCount", LobbyType.Default);
    TypedLobby tdm = new TypedLobby("tdm", LobbyType.Default);
    TypedLobby noRespawn = new TypedLobby("noRespawn", LobbyType.Default);


    private string levelName = "";

    public void Join_KillCount()
    {
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(killCount);
    }

    public void Join_TDM()
    {
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(tdm);
    }

    public void Join_NoRespawn()
    {
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(noRespawn);
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Joined random fail, creating new room");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6;

        PhotonNetwork.CreateRoom("Arena"+Guid.NewGuid().ToString(), roomOptions);
    }

    public override void OnJoinedRoom()
    {
        roomNumberText.text = PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.LoadLevel(levelName);
    }
}
