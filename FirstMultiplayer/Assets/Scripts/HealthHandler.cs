using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [SerializeField] private WeaponChange weaponChange;
    [SerializeField]private AnimationManager animationManager;
    [SerializeField] private WeaponChange weaponController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private AimLookAtRef aimLookAtRef;
   // [SerializeField] private Collider playerCollider;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Awake()
    {
        photonView =GetComponent<PhotonView>();
        animationManager = GetComponent<AnimationManager>();
        weaponController = GetComponent<WeaponChange>();
        playerMovement = GetComponent<PlayerMovement>();

       // playerCollider = GetComponent<Collider>();
    }

    public void DoDamage(int health, int viewId)
    {
        if (photonView.ViewID != viewId) return;

        if (health <= 0)
        {
            playerMovement.SetActive(false, false);
            weaponChange.SetActive(false);           
            aimLookAtRef.SetACtive(false);
            animationManager.SetActive(false);

            animationManager.SetDeadAnimation(true);
        }
    }

    public void ShowKillMessage(int hittedViewId, int shooterViewId)
    {
        photonView.RPC("KillMessageRequest", RpcTarget.All, hittedViewId, shooterViewId);
        
    }

    [PunRPC]
    void KillMessageRequest(int hittedViewId, int shooterViewId)
    {
        Debug.Log(photonView.ViewID);
        FindFirstObjectByType<KillCounts>().ShowKillMessage(hittedViewId, shooterViewId);
    }
}
