using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayColor : MonoBehaviour
{
    [SerializeField] private ColorById[] colorById;
    [SerializeField] private Renderer render;

    private PlayerNamesHud playerNamesHud;

    private void Awake()
    {
        playerNamesHud = FindAnyObjectByType<PlayerNamesHud>();
    }

    public void ChooseColor(int phothonViewId, int colorId)
    {
        GetComponent<PhotonView>().RPC("SetColor", RpcTarget.AllBuffered, phothonViewId, colorId);

        GetComponent<PhotonView>().RPC("NewPlayerHudStats", RpcTarget.AllBuffered, phothonViewId, colorId);
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
    

    [Serializable]
    private struct ColorById
    {
        public int id;
        public Color color;
    }
}
