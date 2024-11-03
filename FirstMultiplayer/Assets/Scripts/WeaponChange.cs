using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class WeaponChange : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private TwoBoneIKConstraint lefthand;
    [SerializeField] private TwoBoneIKConstraint rightHand;
    [SerializeField] private RigBuilder rig;
    [SerializeField] private WeaponRigConfig[] weaponRigConfigs;
    [SerializeField] private LayerMask groundLayer;
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

    private RaycastHit[] hitResults;
    private bool running = false;

    private void OnDestroy()
    {
        if (inputController)
        {
            inputController.OnWeaponChange -= WeaponChangeInput;
            inputController.OnShootInputUpdate -= OnShoot;
        }
    }

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine == true)
        {
            hitResults = new RaycastHit[3];
            inputController = FindFirstObjectByType<InputController>();

            if (inputController)
            {
                inputController.OnWeaponChange += WeaponChangeInput;
                inputController.OnShootInputUpdate += OnShoot;
            }

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

    private void OnShoot(bool shooting)
    {
        if (!active) return;
        if (shooting)
        {
            running = true;
            float x = Mouse.current.position.x.ReadValue();
            float y = Mouse.current.position.y.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));

            int hits = Physics.RaycastNonAlloc(ray, hitResults, 50);

            Debug.Log("Shooting");
            Debug.Log(hits);
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {   
                   
                    Collider collider = hitResults[i].collider;

                    Debug.Log(collider.name); 

                    //if ((groundLayer & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
                    //{
                    //    break;
                    //}

                    PhotonView pV= collider.GetComponent<PhotonView>();

                    if(pV != null)
                    {
                        if (pV == photonView) continue;

                        pV.GetComponent<DisplayColor>().DoDamage(pV.ViewID, photonView.ViewID, weaponRigConfigs[weaponIndex].damage);
                        break;
                    }

                }
            }

            photonView.RPC("MuzzleFlash", RpcTarget.All, photonView.ViewID, weaponIndex, true);
            //StartCoroutine(MuzzleFlashDelay(weaponIndex));
            // photonView.RPC("ShootSound", RpcTarget.All);    
        }
    }

    private void OnDrawGizmos()
    {
        if (!running) return;
        float x = Mouse.current.position.x.ReadValue();
        float y = Mouse.current.position.y.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
        Gizmos.DrawRay(ray);
        
    }

    [PunRPC]
    public void MuzzleFlash(int viewId, int weaponId, bool active)
    {
        if (this.photonView.ViewID == viewId)
        {
            if (active)
            {
                this.weaponRigConfigs[weaponId].flash.SetActive(active);
                StartCoroutine(MuzzleFlashDelay(weaponId));
            }
            else
            {
                this.weaponRigConfigs[weaponId].flash.SetActive(active);
            }
        }
    }


    IEnumerator MuzzleFlashDelay(int weaponId)
    {
        yield return new WaitForSeconds(0.15f);
        photonView.RPC("MuzzleFlash", RpcTarget.All, photonView.ViewID, weaponId, false);
        yield break;
    }


    private void WeaponChangeInput()
    {
        if (!active) return;
        if (weaponIcon == null || ammoAmountText == null) return;

        weaponIndex++;

        if (weaponIndex > weaponRigConfigs.Length - 1)
            weaponIndex = 0;

        weaponIcon.sprite = weaponRigConfigs[weaponIndex].weaponIcon;
        ammoAmountText.text = weaponRigConfigs[weaponIndex].ammoAmount.ToString();

        photonView.RPC("MuzzleFlash", RpcTarget.All, photonView.ViewID, weaponIndex, false);
        photonView.RPC("ChangeWeapon", RpcTarget.AllBuffered, weaponIndex);

        //weaponIndex++;

        //if (weaponIndex > weaponRigConfigs.Length - 1)
        //    weaponIndex = 0;

        //SetNewWeapon(weaponIndex);
    }

    public void SetActive(bool active)
    {
        this.active = active;
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
        public GameObject flash;
        public AudioSource _audio;
        public Transform leftHandTarget;
        public Transform rightHandTarget;
        public Sprite weaponIcon;
        public int ammoAmount;
        public float distance;
        public float damage;
    }
}
