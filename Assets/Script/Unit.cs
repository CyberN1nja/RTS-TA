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
    private AttackController attackController;

    private bool isDead = false;

    void Start()
    {
        unitHealth = unitMaxHealth;

        attackController = GetComponent<AttackController>();
        if (attackController == null)
        {
            Debug.LogWarning($"[Unit] ⚠️ AttackController tidak ditemukan pada {gameObject.name}. Ini mungkin unit non-tempur.");
        }

        if (healthTracker == null)
            healthTracker = GetComponentInChildren<HealthTracker>();

        if (healthTracker != null)
            healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if (UnitSelectionManager.Instance != null)
            UnitSelectionManager.Instance.allUnitList.Add(gameObject);

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (animator == null || navMeshAgent == null)
            Debug.LogWarning("Animator atau NavMeshAgent belum disetel di " + gameObject.name);

        // ✅ Registrasi ke GameManager hanya jika ada AttackController
        if (GameManager.Instance != null && attackController != null)
        {
            if (attackController.isPlayer)
                GameManager.Instance.RegisterPlayerUnit();
            else
                GameManager.Instance.RegisterEnemy();
        }

        Debug.Log($"{gameObject.name} terdeteksi. isPlayer = {(attackController != null ? attackController.isPlayer.ToString() : "N/A")}");
    }

    private void Update()
    {
        if (isDead || navMeshAgent == null || animator == null) return;

        if (navMeshAgent.isOnNavMesh)
        {
            bool isMoving = navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
            animator.SetBool("isMoving", isMoving);
        }
        else
        {
            animator.SetBool("isMoving", false);
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
            healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if (unitHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (GameManager.Instance != null && attackController != null)
        {
            if (attackController.isPlayer)
                GameManager.Instance.UnregisterPlayerUnit();
            else
                GameManager.Instance.UnregisterEnemy();
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("die");
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUnitDeathSound();

        if (UnitSelectionManager.Instance != null)
            UnitSelectionManager.Instance.allUnitList.Remove(gameObject);

        Debug.Log($"💀 {gameObject.name} mati. isPlayer = {attackController?.isPlayer}");

        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        float delay = 2f;

        if (SoundManager.Instance != null && SoundManager.Instance.unitDeathSound != null)
            delay = SoundManager.Instance.unitDeathSound.length;

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (UnitSelectionManager.Instance != null)
            UnitSelectionManager.Instance.allUnitList.Remove(gameObject);
    }
}
