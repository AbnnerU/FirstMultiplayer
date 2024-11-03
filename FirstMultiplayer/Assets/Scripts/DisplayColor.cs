using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayColor : MonoBehaviour
{
    [SerializeField] private ColorById[] colorById;
    [SerializeField] private Renderer render;
    private InputController inputController;

    private PlayerNamesHud playerNamesHud;
    private PlayerCheck playerCheck;

    private void Awake()
    {
        playerNamesHud = FindAnyObjectByType<PlayerNamesHud>();

        if (GetComponent<PhotonView>().IsMine == true)
        {
            inputController = FindFirstObjectByType<InputController>();

            if (inputController)         
                inputController.OnExitLobbyPressed += ExitLobby;  
        }

        playerCheck = FindFirstObjectByType<PlayerCheck>(); 
    }

    public void DoDamage(int hittedViewId, int shooterViewId, float damage)
    {
        playerNamesHud.TryUpdatePlayerHealthStats(hittedViewId, shooterViewId, damage);
    }

    private void OnDestroy()
    {
        if (inputController)
            inputController.OnExitLobbyPressed -= ExitLobby;
    }

    private void ExitLobby()
    {
        Debug.Log("--Exit lobby input--");
        if(playerCheck.IsWaitingScreenActive() == false)
        {
            RemoveData();
            ExitRoom();
        }
    }

    void RemoveData()
    {
        GetComponent<PhotonView>().RPC("RemoveMe", RpcTarget.All, GetComponent<PhotonView>().ViewID);
    }

    void ExitRoom()
    {
        StartCoroutine(GetReadyToLeave());
    }

    public void ChooseColor(int phothonViewId, int colorId)
    {
        GetComponent<PhotonView>().RPC("SetColor", RpcTarget.AllBuffered, phothonViewId, colorId);

        GetComponent<PhotonView>().RPC("NewPlayerHudStats", RpcTarget.All, phothonViewId, colorId);
    }

    [PunRPC]
    void SetColor(int viewId, int colorId)
    {
        Debug.Log("Set color ("+colorId+") in the character ("+viewId+")");
        PhotonView view = this.GetComponent<PhotonView>();

        if(view.ViewID == viewId)
        {
            this.render.material.color = this.colorById[colorId].color;
        }
    }


    [PunRPC]
    void NewPlayerHudStats(int viewId, int colorId)
    {
        PhotonView view = this.GetComponent<PhotonView>();
     
        if (view.ViewID == viewId)
        {
            Debug.Log("Try set color (" + colorId + ") and player (" + viewId + ") on hud");
            this.playerNamesHud.SetNewPlayer(view.Owner.NickName, this.colorById[colorId].color, viewId);
        }
    }

    [PunRPC]
    void RemoveMe(int viewId)
    {
        PhotonView view = this.GetComponent<PhotonView>();

        if (view.ViewID == viewId)
        {
            Debug.Log("Try remove player (" + viewId + ") on hud");
            this.playerNamesHud.RemovePlayer(viewId);
        }
    }

    IEnumerator GetReadyToLeave()
    {
        yield return new WaitForSeconds(1);
        playerNamesHud.Leaving();
        Cursor.visible = true;
        PhotonNetwork.LeaveRoom();

    }


    [Serializable]
    private struct ColorById
    {
        public int id;
        public Color color;
    }
}
