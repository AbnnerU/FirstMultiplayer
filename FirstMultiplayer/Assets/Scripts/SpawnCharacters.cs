using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class SpawnCharacters : MonoBehaviour
{
    [SerializeField] private GameObject character;
    [SerializeField] private Transform[] spawnPoint;

    [SerializeField] private WeaponSpawnConfig[] weaponsSpawnConfig;
    [SerializeField] private float weaponSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(character.name, 
                spawnPoint[PhotonNetwork.CurrentRoom.PlayerCount - 1].position, 
                spawnPoint[PhotonNetwork.CurrentRoom.PlayerCount - 1].rotation);
        }
    }


    public void SpawnWeaponsStart()
    {
        for(int i = 0; i < weaponsSpawnConfig.Length; i++)
        {
            PhotonNetwork.Instantiate(weaponsSpawnConfig[i].weaponRef.name,
                weaponsSpawnConfig[i].weaponPoint.position,
                weaponsSpawnConfig[i].weaponPoint.rotation);
        }
    }

    [Serializable]
    private struct WeaponSpawnConfig
    {
        public Transform weaponPoint;
        public GameObject weaponRef;
    }
    
}
