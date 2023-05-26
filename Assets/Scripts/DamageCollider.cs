using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageCollider : MonoBehaviour
{
    // Or WeaponItems
    // which have a collider - then gets activated depending on action
    // Maybe make a Enum for the different types 

    //      - Arm Left
    //      - Arm Right

    //      - Foot Left
    //      - Foot Right

    [SerializeField] private ParticleSystem hitParticleSystem;
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private float damageAmount = 10f;
    private new Collider collider;

    void Start()
    {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
        DisableCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get the highest parent object
        var highestParent = other.transform.root.gameObject;

        // Check if the highest parent has the specified tag
        if (highestParent.CompareTag(other.tag))
            return;

        // Check the tag of the object that enters the trigger
        if (other.gameObject.CompareTag(targetTag))
        {
            var damageable = other.GetComponentInParent<CharacterManager>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);

                Debug.LogWarning("Hit " + other.gameObject.name);
                Hit(other.transform);
            }
        }
    }

    private void Hit(Transform hitTransform)
    {
        hitParticleSystem.transform.position = hitTransform.position;
        hitParticleSystem.transform.parent = null;
        hitParticleSystem.Play();
    }

    public void EnableCollider()
    {
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    #region DarkSoulsProject

    //
    //     private void OnTriggerEnter(Collider collision)
    // {
    //     Debug.Log("[Info] Attack registered on:  " + collision.name + " from " + gameObject.name);
    //
    //     if ( collision.CompareTag($"Player") )
    //     {
    //         PlayerStats playerStats = collision.GetComponent<PlayerStats>();
    //         CharacterManager enemyCharacterManager = collision.GetComponent<CharacterManager>();
    //
    //         BlockingCollider shield = collision.GetComponentInChildren<BlockingCollider>();
    //
    //         if ( enemyCharacterManager != null )
    //         {
    //             if ( enemyCharacterManager.isParrying )
    //             {
    //                 characterManager.GetComponentInChildren<AnimatorManager>()
    //                     .PlayTargetAnimation("[Combat Action] Parried", true);
    //                 return;
    //             }
    //             else if ( shield != null && enemyCharacterManager.isBlocking )
    //             {
    //                 float physicalDamageAfterBlock = currentWeaponDamage - (currentWeaponDamage * shield.blockingPhysicalDamageAbsorption / 100);
    //
    //                 if ( playerStats != null )
    //                 {
    //                     playerStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "[Combat Action] Blocking Hit");
    //
    //                 }
    //             }
    //             else
    //             {
    //                 if ( playerStats != null )
    //                 {
    //                     // Todo: interface in Stats
    //                     playerStats.TakeDamage(currentWeaponDamage);
    //                 }
    //             }
    //         }
    //     }
    //
    //     if ( collision.CompareTag($"Enemy") )
    //     {
    //         EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
    //         CharacterManager enemyCharacterManager = collision.GetComponent<CharacterManager>();
    //
    //         BlockingCollider shield = collision.GetComponentInChildren<BlockingCollider>();
    //
    //         if ( enemyCharacterManager != null )
    //         {
    //             if ( enemyCharacterManager.isParrying )
    //             {
    //                 characterManager.GetComponentInChildren<AnimatorManager>()
    //                     .PlayTargetAnimation("[Combat Action] Parried", true);
    //             }
    //             else if ( shield != null && enemyCharacterManager.isBlocking )
    //             {
    //                 float physicalDamageAfterBlock = currentWeaponDamage - (currentWeaponDamage * shield.blockingPhysicalDamageAbsorption / 100);
    //
    //                 if ( enemyStats != null )
    //                 {
    //                     enemyStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "[Combat Action] Blocking Hit");
    //
    //                 }
    //             }
    //             else
    //             {
    //                 if ( enemyStats != null )
    //                 {
    //                     // Todo: interface in Stats
    //                     enemyStats.TakeDamage(currentWeaponDamage);
    //                 }
    //             }
    //         }
    //     }
    // }

    #endregion
}