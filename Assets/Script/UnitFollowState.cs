using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    NavMeshAgent agent;
    public float attackingDistance = 1f;

    // Dipanggil saat masuk ke state Follow
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
        attackController.SetFollowMaterial();
    }

    // Dipanggil setiap frame saat berada di state Follow
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UnitMovement unitMovement = animator.transform.GetComponent<UnitMovement>();

        // Jika tidak ada target → kembali ke idle
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
            return; // keluar supaya tidak lanjut
        }

        // Jika target ada dan unit tidak sedang dikomando untuk gerak bebas → kejar target
        if (unitMovement.isCommandedToMove == false)
        {
            agent.SetDestination(attackController.targetToAttack.position);
            animator.transform.LookAt(attackController.targetToAttack);

            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
            if (distanceFromTarget < attackingDistance)
            {
                agent.SetDestination(animator.transform.position);
                animator.SetBool("isAttacking", true); // masuk ke state attack
            }
        }
        else
        {
            // Jika player klik tempat lain → batal follow dan kembali ke idle
            attackController.targetToAttack = null;
            animator.SetBool("isFollowing", false);
        }
    }
}
