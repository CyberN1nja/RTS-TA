using UnityEngine;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour
{
    public Sprite availableSprite;
    public Sprite unAvailableSprite;

    private bool isAvailable;

    public BuySystem buySystem;

    public int databaseItemID;

    private bool hasBeenUnlocked = false;

    private void Start()
    {
        gameObject.SetActive(false);
        // Subscribe to event / Listen to event
        ResourceManager.Instance.OnResourcesChanged += HandleResoucesChanged;
        HandleResoucesChanged();
        ResourceManager.Instance.OnBuildingsChanged += HandleBuildingsChanged;
        HandleBuildingsChanged();
    }

    public void ClickedOnSlot()
    {
        if (isAvailable)
        {
            buySystem.placementSystem.StartPlacement(databaseItemID);
        }
    }

    private void UpdateAvailabilityUI()
    {
        if (this == null) return; // Pastikan objek ini masih ada sebelum melanjutkan

        if (isAvailable)
        {
            GetComponent<Image>().sprite = availableSprite;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Image>().sprite = unAvailableSprite;
            GetComponent<Button>().interactable = false;
        }
    }

    private void HandleResoucesChanged()
    {
        if (this == null) return; // Pastikan objek ini masih ada sebelum melanjutkan

        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

        bool requirmentMet = true;

        foreach (BuildRequirement req in objectData.resouceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                requirmentMet = false;
                break;
            }
        }

        isAvailable = requirmentMet;

        UpdateAvailabilityUI();
    }

    private void HandleBuildingsChanged()
    {
        if (this == null) return; // Pastikan objek ini masih ada sebelum melanjutkan

        if (hasBeenUnlocked)
            return; // Sudah pernah aktif, jangan cek lagi

        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

        foreach (BuildingType dependency in objectData.buildDependency)
        {
            if (dependency == BuildingType.None)
            {
                gameObject.SetActive(true);
                hasBeenUnlocked = true;
                return;
            }

            if (!ResourceManager.Instance.allExistingBuildings.Contains(dependency))
            {
                return; // Dependency belum terpenuhi, jangan aktifkan
            }
        }

        // Semua dependency terpenuhi
        gameObject.SetActive(true);
        hasBeenUnlocked = true; // Ingat bahwa tombol sudah pernah diaktifkan
    }
}
