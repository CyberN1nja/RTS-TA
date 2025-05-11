using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IDamageable
{
    private float unitHealth;
    public float unitMaxHealth;

    public HealthTracker healthTracker;

    Animator animator;
    NavMeshAgent navMeshAgent;

    void Start()
    {
        UnitSelectionManager.Instance.allUnitList.Add(gameObject);

        unitHealth = unitMaxHealth;
        UpdateHealthUI();

        animator = GetComponent<Animator>();    
        navMeshAgent = GetComponent<NavMeshAgent>();
        Debug.Log( "ini animator yang di pakaii" + animator.runtimeAnimatorController.name);
    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitList.Remove(gameObject);
       
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if (unitHealth <= 0)
        {
            // Dying logic 

            // Destruction  or Dying Animation 

            // Dying sound Effect 

            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damageToInflict)
    {
        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
