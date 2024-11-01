using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Renderer[] render;
    [SerializeField] private Collider colliderRef;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float respawnTime = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            photonView.RPC("PlayPickupAudio", RpcTarget.All);
            photonView.RPC("TurnOffWeaponPickupMesh", RpcTarget.All);
        }
    }

    [PunRPC]
    void PlayPickupAudio()
    {
        audioSource.Play();
    }

    [PunRPC]
    void TurnOffWeaponPickupMesh()
    {
        for(int i = 0; i < render.Length; i++)
            render[i].enabled = false;

        colliderRef.enabled = false;
        StartCoroutine(RespawnDelay());
    }

    [PunRPC]
    void TurnOnWeaponPickupMesh()
    {
        for (int i = 0; i < render.Length; i++)
            render[i].enabled = true;

        colliderRef.enabled = true;
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        photonView.RPC("TurnOnWeaponPickupMesh", RpcTarget.All);
    }
}
