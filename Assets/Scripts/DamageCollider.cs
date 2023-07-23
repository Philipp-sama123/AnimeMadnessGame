using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
public class DamageCollider : MonoBehaviour
{
    //      - Arm Left
    //      - Arm Right
    //      - Foot Left
    //      - Foot Right

    [SerializeField] private ParticleSystem hitParticleSystem;
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private float damageAmount = 10f;
    private new Collider collider;
    private void Start()
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
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);

                Debug.LogWarning("Hit " + other.gameObject.name);
                Debug.LogWarning("Hit " + other.transform.localPosition.ToString());
                Hit(other.transform.localPosition);
            }
        }
    }

    private void Hit(Vector3 hitPosition)
    {
        hitParticleSystem.transform.position = hitPosition;
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
}