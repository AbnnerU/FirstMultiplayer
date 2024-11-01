using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class WeaponChange : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint lefthand;
    [SerializeField] private TwoBoneIKConstraint rightHand;
    [SerializeField] private RigBuilder rig;
    [SerializeField] private WeaponRigConfig[] weaponRigConfigs;
   // [SerializeField] private MultiAimConstraint[] aimObjects;
    

    private CinemachineVirtualCamera _virtualCamera;
    private GameObject camObject;

    //private Transform aimTarget;

    private PhotonView photonView;

    private InputController inputController;
    private int weaponIndex = -1;
    private int currentWeaponIndex = -1;

    private SpawnCharacters testForWeapons;

    private Image weaponIcon;
    private TMP_Text ammoAmountText;

    private void OnDestroy()
    {
        if (inputController)
            inputController.OnWeaponChange -= WeaponChangeInput;
    }

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine == true)
        {
            inputController = FindFirstObjectByType<InputController>();

            if (inputController)
                inputController.OnWeaponChange += WeaponChangeInput;

            camObject = GameObject.FindGameObjectWithTag("PlayerCam");

            _virtualCamera = camObject.GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = transform;
            _virtualCamera.LookAt = transform;

            ammoAmountText = GameObject.FindGameObjectWithTag("AmmoAmount").GetComponent<TMP_Text>();
            weaponIcon = GameObject.FindGameObjectWithTag("WeaponIcon").GetComponent<Image>();

            //aimTarget = GameObject.FindGameObjectWithTag("AimTarget").transform;

            //Invoke("SetLookAt", 0.1f);
        }
        else
        {    
            GetComponent<PlayerMovement>().SetActive(false, true);
            GetComponent<PlayerMovement>().enabled = false;

            GetComponent<AnimationManager>().Disable();
            GetComponent<AnimationManager>().enabled = false;
        }

        GameObject haveWeapon = GameObject.FindGameObjectWithTag("WeaponPickup");

        if (haveWeapon == null)
        {
            testForWeapons = FindFirstObjectByType<SpawnCharacters>();
            testForWeapons.SpawnWeaponsStart();
        }
    }

    //void SetLookAt()
    //{
    //    if(aimTarget != null)
    //    {
    //        for(int i = 0; i < aimObjects.Length; i++)
    //        {
    //            var target = aimObjects[i].data.sourceObjects;
    //            target.SetTransform(0, aimTarget);
    //            aimObjects[i].data.sourceObjects = target;
    //        }
    //        rig.Build();
    //    }
    //}

    private void Start()
    {
        //photonView.RPC("ChangeWeapon", RpcTarget.MasterClient);
        WeaponChangeInput();
    }

    private void WeaponChangeInput()
    {
        if (weaponIcon==null || ammoAmountText==null) return;

        weaponIndex++;

        if (weaponIndex > weaponRigConfigs.Length - 1)
            weaponIndex = 0;

        weaponIcon.sprite = weaponRigConfigs[weaponIndex].weaponIcon;
        ammoAmountText.text = weaponRigConfigs[weaponIndex].ammoAmount.ToString();

        photonView.RPC("ChangeWeapon", RpcTarget.AllBuffered, weaponIndex);

        //weaponIndex++;

        //if (weaponIndex > weaponRigConfigs.Length - 1)
        //    weaponIndex = 0;

        //SetNewWeapon(weaponIndex);
    }

    //public void SetNewWeapon(int index)
    //{
    //    if (currentWeaponIndex >= 0)
    //        weaponRigConfigs[currentWeaponIndex].weapon.SetActive(false);

    //    weaponRigConfigs[index].weapon.SetActive(true);
    //    lefthand.data.target = weaponRigConfigs[index].leftHandTarget;
    //    rightHand.data.target = weaponRigConfigs[index].rightHandTarget;

    //    currentWeaponIndex = index;

    //    rig.Build();
    //}

    [PunRPC]
    public void ChangeWeapon(int index)
    {  
        if (currentWeaponIndex >= 0)
            weaponRigConfigs[currentWeaponIndex].weapon.SetActive(false);

        weaponRigConfigs[index].weapon.SetActive(true);
        lefthand.data.target = weaponRigConfigs[index].leftHandTarget;
        rightHand.data.target = weaponRigConfigs[index].rightHandTarget;

        currentWeaponIndex = index;

        rig.Build();
    }

    [Serializable]
    private class WeaponRigConfig
    {
        public GameObject weapon;
        public Transform leftHandTarget;
        public Transform rightHandTarget;
        public Sprite weaponIcon;
        public int ammoAmount;
    }
}
