using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size
            );
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        Debug.Log("[PlacementState] OnAction dipanggil di posisi grid: " + gridPosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            Debug.LogWarning("[PlacementState] Invalid placement!");
            return;
        }

        GameObject prefab = database.objectsData[selectedObjectIndex].Prefab;

        if (prefab == null)
        {
            Debug.LogError("[PlacementState] Prefab NULL untuk ID: " + database.objectsData[selectedObjectIndex].ID);
            return;
        }

        // ✅ Pastikan kita bersihkan data lama jika ada
        RemovePreviousObjectData(gridPosition);

        // ✅ Tempatkan objek
        int index = objectPlacer.PlaceObject(prefab, grid.CellToWorld(gridPosition));

        ResourceManager.Instance.DecreaseResourcesBasedOnRequirement(database.objectsData[selectedObjectIndex]);

        BuildingType buildingType = database.objectsData[selectedObjectIndex].thisBuildingType;
        ResourceManager.Instance.UpdateBuildingChanged(buildingType, true, new Vector3());

        // ✅ Tambahkan data ke Grid
        GridData selectedData = floorData;
        selectedData.AddObjectAt(
            gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index
        );

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    // 🧹 Membersihkan data sebelumnya di grid (jika ada)
    private void RemovePreviousObjectData(Vector3Int gridPosition)
    {
        if (floorData.HasObjectAt(gridPosition))
        {
            floorData.RemoveObjectAt(gridPosition);
        }
        else
        {
            Debug.Log("[PlacementState] Tidak ada objek sebelumnya untuk dihapus di posisi: " + gridPosition);
        }
    }


    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = floorData;
        Vector2Int size = database.objectsData[selectedObjectIndex].Size;

        bool canPlace = selectedData.CanPlaceObjectAt(gridPosition, size);
        if (!canPlace)
        {
            Debug.LogWarning("[CheckPlacementValidity] Tidak bisa tempatkan karena GridData menolak posisi.");
            return false;
        }

        // Tambahan: Cek apakah ini area yang valid untuk unit (misal tag-nya "SpawnArea")
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Collider[] hits = Physics.OverlapBox(worldPosition, new Vector3(0.5f, 0.5f, 0.5f));
        bool foundSpawnArea = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("SpawnArea")) // pastikan GameObject area hijau diberi tag ini
            {
                foundSpawnArea = true;
                break;
            }
        }

        if (!foundSpawnArea)
        {
            Debug.LogWarning("[CheckPlacementValidity] Hanya bisa spawn di area bertag 'SpawnArea'");
            return false;
        }

        // Cek tumpang tindih dengan Unit/Enemy
        int layerMask = LayerMask.GetMask("Obstacle");
        Vector3 boxHalfExtents = new Vector3(0.4f, 0.5f, 0.4f);

        Collider[] colliders = Physics.OverlapBox(worldPosition, boxHalfExtents, Quaternion.identity, layerMask);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Unit") || collider.CompareTag("Enemy"))
            {
                Debug.LogWarning("[CheckPlacementValidity] Tabrakan dengan Unit/Enemy.");
                return false;
            }
        }

        return true;
    }


    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
