using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AimLookAtRef : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private PhotonView photonView;
    private Transform aimTarget;
    private Transform _transform;

    void Start()
    {
        aimTarget = GameObject.FindGameObjectWithTag("AimTarget").transform;
        _transform = transform;
    }

    public void SetACtive(bool active)
    {
        this.active = active;
    }

    void FixedUpdate()
    {
        if (!active) return;

        if (photonView.IsMine == true)
            _transform.position = aimTarget.position;
        else
            this.enabled = false;
    }
}
