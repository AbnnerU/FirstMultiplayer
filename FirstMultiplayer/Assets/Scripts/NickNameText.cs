using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NickNameText : MonoBehaviour
{
    [SerializeField] private TMP_Text nickNameText;

    // Start is called before the first frame update
    void Start()
    {
        nickNameText.text = PhotonNetwork.LocalPlayer.NickName;
    }
    
}
