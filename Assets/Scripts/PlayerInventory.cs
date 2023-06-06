using System.Collections.Generic;
using _Items;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    private MainPlayerManager mainPlayerManager;
    [Header("Quick Slot items")]
    public WeaponItem rightWeapon = null;
    public WeaponItem leftWeapon = null;

    public WeaponSlot leftHandSlot;
    public WeaponSlot rightHandSlot;
    public WeaponSlot rightFootSlot;
    public WeaponSlot leftFootSlot;
    
    public WeaponItem attackingWeapon;
    public DamageCollider leftHandDamageCollider;
    public DamageCollider rightHandDamageCollider;
    public DamageCollider leftFootDamageCollider;
    public DamageCollider rightFootDamageCollider;

    private Animator animator;

    private void Awake()
    {
        mainPlayerManager = GetComponentInParent<MainPlayerManager>();
        // playerInventory = GetComponentInParent<PlayerInventory>();
        // playerStats = GetComponentInParent<PlayerStats>();
        // inputHandler = GetComponentInParent<InputHandler>();

        animator = GetComponent<Animator>();


        WeaponSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponSlot>();

        foreach (WeaponSlot weaponSlot in weaponHolderSlots)
        {
            if (weaponSlot.isLeftHandSlot)
            {
                leftHandSlot = weaponSlot;
            }
            else if (weaponSlot.isRightHandSlot)
            {
                rightHandSlot = weaponSlot;
            }
        }
    }

    public void LoadBothWeaponsOnSlot()
    {
        // LoadWeaponOnSlot(playerInventory.rightWeapon, true);
        // LoadWeaponOnSlot(playerInventory.leftWeapon, true);
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.currentWeapon = weaponItem;
            leftHandSlot.LoadWeaponModel(weaponItem);
            LoadLeftWeaponDamageCollider();
            // quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);

            #region Handle Left Weapon Idle Animations

            //
            // if (weaponItem != null)
            // {
            //     animator.CrossFade(weaponItem.leftHandIdle01, 0.2f);
            // }
            // else
            // {
            //     animator.CrossFade("Left Arm Empty", 0.2f);
            // }

            #endregion
        }

        if (!isLeft)
        {
            rightHandSlot.currentWeapon = weaponItem;
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();

            #region Handle Left Weapon Idle Animations

            // if (weaponItem != null)
            // {
            //     animator.CrossFade(weaponItem.rightHandIdle01, 0.2f);
            // }
            // else
            // {
            //     animator.CrossFade("Right Arm Empty", 0.2f);
            // }

            #endregion
        }
    }

    #region Handle Weapon's Damage Collider

    private void LoadLeftWeaponDamageCollider()
    {
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    private void LoadRightWeaponDamageCollider()
    {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    public void OpenDamageCollider()
    {
        if (leftHandSlot)
            rightHandDamageCollider.EnableCollider();
        if (rightHandSlot)
            leftHandDamageCollider.EnableCollider();
        if (leftFootSlot)
            leftFootDamageCollider.EnableCollider();
        if (rightFootSlot)
            rightFootDamageCollider.EnableCollider();

        if (!rightFootSlot && !leftFootSlot && !rightHandSlot && !leftHandSlot)
            Debug.LogWarning("[Error] No Weapon defined!");
    }

    public void CloseDamageCollider()
    {
        if (leftHandSlot)
            rightHandDamageCollider.DisableCollider();
        if (rightHandSlot)
            leftHandDamageCollider.DisableCollider();
        if (leftFootSlot)
            leftFootDamageCollider.DisableCollider();
        if (rightFootSlot)
            rightFootDamageCollider.DisableCollider();

        if (!rightFootSlot && !leftFootSlot && !rightHandSlot && !leftHandSlot)
            Debug.LogWarning("[Error] No Weapon defined!");
    }

    #endregion

    private void Start()
    {
        if (rightWeapon != null)
            LoadWeaponOnSlot(rightWeapon, false);
        if (leftWeapon != null)
            LoadWeaponOnSlot(leftWeapon, true);
    }
}