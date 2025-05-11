using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public int credits = 300;

    public event Action OnResourcesChanged;

    public event Action OnBuildingsChanged;


    // garis bawahi
    public TextMeshProUGUI creditsUI;

    public List<BuildingType> allExistingBuildings;

    public PlacementSystem placementSystem;

    public int creditsPerKiloSpice = 7;

    public enum ResourcesType
    {
        Credits
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateBuildingChanged(BuildingType buildingType, bool isNew, Vector3 position)
    {
        if (isNew)
        {
            allExistingBuildings.Add(buildingType);

            SoundManager.Instance.PlayBuildingConstructionSound();
        }
        else
        {
            placementSystem.RemovePlacementData(position);
            allExistingBuildings.Remove(buildingType);
        }

        OnBuildingsChanged?.Invoke();
    }


    public void IncreaseResource(ResourcesType resource, int amountToIncrease)
    {
        switch (resource)
        {
            case ResourcesType.Credits:
                credits += amountToIncrease;
                break;
            default:
                break;
        }

        OnResourcesChanged?.Invoke();

    }

    public void DecreaseResource(ResourcesType resource, int amountToDecrease)
    {
        switch (resource)
        {
            case ResourcesType.Credits:
                credits -= amountToDecrease;
                break;
            default:
                break;
        }

        OnResourcesChanged?.Invoke();
    }

    public void SellBuilding(BuildingType buildingType)
    {
        Debug.Log("Selling building");

        SoundManager.Instance.PlayBuildingSellingSound();

        var sellingPrice = 0;

        foreach (ObjectData obj in DatabaseManager.Instance.databaseSO.objectsData)
        {
            if (obj.thisBuildingType == buildingType)
            {
                foreach (BuildRequirement req in obj.resouceRequirements)
                {
                    if (req.resource == ResourcesType.Credits)
                    {
                        sellingPrice = req.amount;
                    }
                }
            }
        }

        int amountToReturn = (int)(sellingPrice * 0.50f);
        
        IncreaseResource(ResourcesType.Credits, amountToReturn);
    }

    private void UpdateUI()
    {
        creditsUI.text = $"{credits}";
    }

    public int GetCredits()
    {
        return credits;
    }

    internal int GetResourceAmount(ResourcesType resource)
    {
        switch (resource)
        {
            case ResourcesType.Credits:
                return credits;
            default:
                break;
        }

        return 0;
        
    }

    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.resouceRequirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
    }

    private void OnEnable()
    {
        OnResourcesChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OnResourcesChanged -= UpdateUI;
    }

}
