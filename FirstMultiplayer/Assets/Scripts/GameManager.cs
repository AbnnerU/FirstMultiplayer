
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nickNameInputField;
    [SerializeField] private TMP_Text connectingText;

    private string setName = "";


    // Start is called before the first frame update
    void Start()
    {
        connectingText.enabled = false;
    }

    public void SetNickName(string newNickName)
    {
        Debug.Log("Nickname saved" + newNickName);
        setName = newNickName;

        PhotonNetwork.LocalPlayer.NickName = setName;
    }

    public void TryEnterInLobby()
    {
        if(setName == string.Empty || setName!= "")
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();

            connectingText.enabled = true;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED TO THE SERVER (MASTER)");

        SceneManager.LoadScene("Lobby");
        //PhotonNetwork.JoinRandomRoom();
    }
}
