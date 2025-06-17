using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    NavMeshAgent agent;
    public float attackingDistance = 1.3f;

    // Dipanggil saat masuk ke state Follow
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
        attackController.SetFollowMaterial();
    }

    // Dipanggil setiap frame saat berada di state Follow
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UnitMovement unitMovement = animator.transform.GetComponent<UnitMovement>();

        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
            animator.SetBool("isAttacking", false); // tambah ini agar aman
            return;
        }

        if (!unitMovement.isCommandedToMove)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(attackController.targetToAttack.position);
                animator.transform.LookAt(attackController.targetToAttack);

                float distanceFromTarget = Vector3.Distance(
                    attackController.targetToAttack.position,
                    animator.transform.position
                );

                if (distanceFromTarget < attackingDistance)
                {
                    Debug.Log("[ENEMY FOLLOW] Masuk ke jarak serang: " + distanceFromTarget);
                    agent.SetDestination(animator.transform.position);
                    animator.SetBool("isAttacking", true);
                }
            }
            else if (agent != null && !agent.isOnNavMesh)
            {
                Debug.LogWarning("[UnitFollowState] Agent belum berada di atas NavMesh! Posisi: " + agent.transform.position);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(agent.transform.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                    Debug.Log("[UnitFollowState] Agent dipindah ke NavMesh di posisi: " + hit.position);
                }
                else
                {
                    Debug.LogWarning("[UnitFollowState] Tidak ditemukan NavMesh di dekat posisi: " + agent.transform.position);
                }
            }
        }
        else
        {
            attackController.targetToAttack = null;
            animator.SetBool("isFollowing", false);
        }
    }

}
