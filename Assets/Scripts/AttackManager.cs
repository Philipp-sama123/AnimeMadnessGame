using System;
using _Items;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private Animator animator;

    public string lastAttack;
    public bool canDoCombo;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        canDoCombo = animator.GetBool("CanDoCombo");
    }

    public void HandleLightAttackWeaponCombo(WeaponItem weapon)
    {
        if (canDoCombo)
        {
            // animator.SetBool("CanDoCombo", false);
            if (lastAttack == weapon.lightAttack01)
            {
                animator.CrossFade(weapon.lightAttack02, .2f);
                animator.SetBool("IsUsingRootMotion", true);

                lastAttack = weapon.lightAttack02;
            }
            else
            {
                if (lastAttack == weapon.lightAttack02)
                {
                    animator.CrossFade(weapon.lightAttack03, .2f);
                    animator.SetBool("IsUsingRootMotion", true);

                    lastAttack = weapon.lightAttack03;
                }
            }
        }
        else
        {
            playerInventory.attackingWeapon = weapon;

            animator.CrossFade(weapon.lightAttack01, .2f);
            animator.SetBool("IsUsingRootMotion", true);

            lastAttack = weapon.lightAttack01;
        }
    }

    // ToDO: Combat Manager
    private void PerformBlockingAction()
    {
        // if (playerManager.isInteracting)
        //     return;
        // if (playerManager.isBlocking)
        //     return;
        //
        // playerAnimatorManager.PlayTargetAnimation("[Combat Action] Blocking Start", false, true);
        // playerEquipmentManager.OpenBlockingCollider();
        // playerManager.isBlocking = true;
    }
    // put in parent component

    // put in parent component
    public void HandleHeavyAttack()
    {
        animator.SetBool("IsUsingRootMotion", true);
        animator.CrossFade("Attack_2", .2f);
    }

    private void HandleLightAttack()
    {
        animator.SetBool("IsUsingRootMotion", true);
        animator.CrossFade("Attack_1", .2f);
    }
    public void EnableRotation()
    {
        animator.SetBool("CanRotate", true);
    }

    public void StopRotation()
    {
        animator.SetBool("CanRotate", false);
    }

    public void EnableCombo()
    {
        animator.SetBool("CanDoCombo", true);
    }

    public void DisableCombo()
    {
        animator.SetBool("CanDoCombo", false);
    }
}