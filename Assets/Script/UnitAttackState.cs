using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;
    public float stopAttackingDistance = 3f;

    public float attackRate = 2f;
    public float attackTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
        attackController.SetAttackMaterial();
        attackController.muzzleEffect.gameObject.SetActive(true);
        Debug.Log($"[UnitAttackState] OnStateEnter: {animator.name}, Instance ID: {animator.GetInstanceID()}");

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("[ENEMY ATTACK STATE] Aktif - " + animator.name);

        // Periksa apakah UnitMovement ada, untuk keperluan pengecekan player
        bool canAttack = true;

        if (animator.TryGetComponent<UnitMovement>(out var movement))
        {
            // Jika unit dikendalikan player dan sedang bergerak, jangan menyerang
            canAttack = !movement.isCommandedToMove;
        }

        if (attackController.targetToAttack != null && canAttack)
        {
            if (attackController.targetToAttack == null)
            {
                animator.SetBool("isAttacking", false);
                return;
            }

            LookAtTarget();
            // Bisa aktifkan jika ingin bergerak sambil menyerang:
            // agent.SetDestination(attackController.targetToAttack.position);

            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = 1f / attackRate;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }

            // Apakah masih dalam jarak serang?
            float distanceFromTarget = Vector3.Distance(
                attackController.targetToAttack.position,
                animator.transform.position
            );

            Debug.Log("[UnitAttackState] Jarak ke target: " + distanceFromTarget);

            if (distanceFromTarget > stopAttackingDistance || attackController.targetToAttack == null)
            {
                animator.SetBool("isAttacking", false); // kembali ke follow
            }
        }
        else
        {
            animator.SetBool("isAttacking", false); // kembali ke follow
        }
    }


    private void Attack()
    {
        var damageToInflict = attackController.unitDamage;

        SoundManager.Instance.PlayInfantryAttackSound();

        Debug.Log("[ENEMY] Menyerang " + attackController.targetToAttack?.name);

        var damageable = attackController.targetToAttack.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageToInflict);
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController.muzzleEffect.gameObject.SetActive(false);
    }
}
