using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerAttackManager : MonoBehaviour
    {
        private Animator animator;
        private Array damageColliders;
        public string lastAttack;
        public bool canDoCombo;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            damageColliders = GetComponentsInChildren<DamageCollider>();
        }

        private void Update()
        {
            canDoCombo = animator.GetBool("CanDoCombo");
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
            foreach (DamageCollider damageCollider in damageColliders)
            {
                damageCollider.EnableCollider();
            }
            animator.SetBool("IsUsingRootMotion", true);
            animator.CrossFade("Attack_2", .2f);
        }

        public void HandleLightAttack()
        {
            foreach (DamageCollider damageCollider in damageColliders)
            {
                damageCollider.EnableCollider();
            }
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
}