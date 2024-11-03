using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Random = UnityEngine.Random;

public class KillCounts : MonoBehaviour
{
    [SerializeField] private List<Kills> highestKills = new List<Kills>();
    [SerializeField] private KillAndNamesTexts[] textsElements;
    [SerializeField] private GameObject killCountPanel;
    [SerializeField] private PlayerNamesHud namesHud;

    private InputController inputController;
    private bool tabOn = false;

    // Start is called before the first frame update
    void Awake()
    {
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
