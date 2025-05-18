using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Unit : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float unitMaxHealth = 100f;
    private float unitHealth;
    public HealthTracker healthTracker;

    [Header("Components")]
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private bool isDead = false;

    void Start()
    {
        // Register unit to selection manager
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitList.Add(gameObject);
        }

        unitHealth = unitMaxHealth;
        UpdateHealthUI();

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (animator == null || navMeshAgent == null)
        {
            Debug.LogError("Missing Animator or NavMeshAgent on " + gameObject.name);
        }

        if (healthTracker == null)
        {
            healthTracker = GetComponentInChildren<HealthTracker>();
            if (healthTracker == null)
                Debug.LogWarning("HealthTracker not found in Unit: " + gameObject.name);
        }

        if (animator?.runtimeAnimatorController != null)
        {
            Debug.Log("Animator in use: " + animator.runtimeAnimatorController.name);
        }
    }

    private void Update()
    {
        if (isDead || navMeshAgent == null || animator == null) return;

        // 🛡️ Safe check before accessing NavMeshAgent distance
        if (navMeshAgent.isOnNavMesh)
        {
            bool isMoving = navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
            animator.SetBool("isMoving", isMoving);
        }
        else
        {
            animator.SetBool("isMoving", false); // fallback
        }
    }

    public void TakeDamage(int damageToInflict)
    {
        if (isDead) return;

        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthTracker != null)
        {
            healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);
        }

        if (unitHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("die");
        }

        // 🧠 Play death sound via SoundManager
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUnitDeathSound();
        }

        // 🧼 Remove from unit list
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitList.Remove(gameObject);
        }

        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        float delay = 2f;

        if (SoundManager.Instance != null && SoundManager.Instance.unitDeathSound != null)
        {
            delay = SoundManager.Instance.unitDeathSound.length;
        }

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (UnitSelectionManager.Instance != null)
        {
            UnitSelectionManager.Instance.allUnitList.Remove(gameObject);
        }
    }
}
