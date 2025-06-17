using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO database;
    [SerializeField] private GridData floorData, furnitureData;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private int selectedID;
    private IBuildingState buildingState;
    public bool inSellMode;

    private bool hasPlaced = false; // ✅ Cegah double placement

    private void Start()
    {
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        Debug.Log("Should Start Placement");
        selectedID = ID;
        Debug.Log("Placement ID: " + ID);

        StopPlacement();

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void RemovePlacementData(Vector3 position)
    {
        floorData.RemoveObjectAt(grid.WorldToCell(position));
    }

    public void StartRemoving()
    {
        StopPlacement();
        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        inputManager.OnClicked += EndSelling;
        inputManager.OnExit += EndSelling;
    }

    private void EndSelling()
    {
        inSellMode = false;
    }

    private void PlaceStructure()
    {
        if (hasPlaced) return; // ✅ Debounce
        hasPlaced = true;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);

        ObjectData ob = database.GetObjectByID(selectedID);
        foreach (BuildBenefits bf in ob.benefits)
        {
            CalculateAndAddBenefit(bf);
        }

        StopPlacement();

        StartCoroutine(ResetPlaceFlag()); // ✅ Reset flag agar bisa klik lagi
    }

    private IEnumerator ResetPlaceFlag()
    {
        yield return null; // tunggu 1 frame
        hasPlaced = false;
    }

    private void CalculateAndAddBenefit(BuildBenefits bf)
    {
        switch (bf.benefitType)
        {
            case BuildBenefits.BenefitType.Housing:
                // StatusManager.Instance.IncreaseHousing(bf.benefitAmount);
                break;
        }
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;

        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            inSellMode = true;
            StartRemoving();
        }

        if (buildingState == null)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}
