using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{

    public Transform targetToAttack;

    public Material idleStateMaterial;
    public Material followStateMaterial;
    public Material attackStateMaterial;

    public bool isPlayer;

    public int unitDamage;

    public GameObject muzzleEffect;

    private void OnTriggerEnter(Collider other)
    {
        // deteksi enemy
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null) 
        {
            targetToAttack = other.transform;
        }

        // deteksi player penambahan
        if (!isPlayer && other.CompareTag("Unit") && targetToAttack == null)
        {
            Debug.Log("[AttackController] Enemy mendeteksi unit: " + other.name);
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isPlayer && other.CompareTag("Unit") && targetToAttack == null)
        {
            Debug.Log("[Enemy] Deteksi Unit dalam jangkauan lewat TriggerStay: " + other.name);
            targetToAttack = other.transform;
        }

        if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
        {
            Debug.Log("[Unit] Deteksi Enemy dalam jangkauan lewat TriggerStay: " + other.name);
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = null;
        }
    }

    public void SetIdleMaterial()
    {
        // GetComponent<Renderer>().material = idleStateMaterial;
    }

    public void SetFollowMaterial()
    {
        // GetComponent<Renderer>().material = followStateMaterial;
    }

    public void SetAttackMaterial()
    {
        // GetComponent<Renderer>().material = attackStateMaterial;
    }

    private void OnDrawGizmos()
    {
        if (isPlayer)
        {
            // Follow distance 
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 10f * 0.2f);

            // Attack Distance
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);

            // Stop Attack Distance
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 1.2f);
        }
        else
        {
            // === Gizmo untuk ENEMY ===
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 30f * 0.2f); // follow range

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f); // attack range

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 1.2f); // stop attack
        }
    }
}
