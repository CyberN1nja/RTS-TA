using System;
using UnityEngine;
using UnityEngine.AI;

public class Constructable : MonoBehaviour, IDamageable
{
    private float constHealth;
    public float constmaxHealth;

    public HealthTracker healthTracker;

    public bool isEnemy = false;

    public bool inPreviewMode;
    public BuildingType buildingType;
    public Vector3 buildPosition;

    private void Start()
    {
        // Auto-assign jika belum di-set di inspector
        if (healthTracker == null)
        {
            healthTracker = GetComponentInChildren<HealthTracker>();
            if (healthTracker == null)
            {
                Debug.LogError("HealthTracker is missing on " + gameObject.name);
                return;
            }
        }

        constHealth = constmaxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthTracker != null)
        {
            healthTracker.UpdateSliderValue(constHealth, constmaxHealth);
        }

        if (constHealth <= 0)
        {
            ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);
            SoundManager.Instance.PlayBuildingDestructionSound();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (!inPreviewMode && constHealth > 0 && buildPosition != Vector3.zero)
        {
            ResourceManager.Instance.SellBuilding(buildingType);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Bangunan menerima damage: " + damage);
        constHealth -= damage;
        UpdateHealthUI();
    }

    public void ConstructableWasPlaced(Vector3 position)
    {
        buildPosition = position;
        inPreviewMode = false;

        Debug.Log("placed");

        // Auto-assign jika null
        if (healthTracker == null)
        {
            healthTracker = GetComponentInChildren<HealthTracker>();
            if (healthTracker == null)
            {
                Debug.LogError("HealthTracker is missing when placing constructable!");
                return;
            }
        }

        healthTracker.gameObject.SetActive(true);

        ActivateObstacle();

        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        var powerUser = GetComponent<PowerUser>();

        if (powerUser != null)
        {
            powerUser.PowerOn();
        }

        if (buildingType == BuildingType.SupplyCenter)
        {
            GameObject prefab = Resources.Load<GameObject>("Harvester");

            if (prefab != null)
            {
                Transform supplyDropTransform = transform.Find("SupplyDrop").transform;

                GameObject instance = Instantiate(prefab, supplyDropTransform.position, Quaternion.identity);
                instance.transform.SetParent(null);

                Harvester harvester = instance.GetComponent<Harvester>();
                harvester.supplyCenter = supplyDropTransform;
            }
        }
    }

    private void ActivateObstacle()
    {
        NavMeshObstacle[] obstacles = GetComponentsInChildren<NavMeshObstacle>();
        foreach (NavMeshObstacle obstacle in obstacles)
        {
            obstacle.enabled = true;
        }
    }
}
