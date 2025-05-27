using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int Id, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, Id, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception("Dictionary already contains this cell position");
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal1 = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal1.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal1;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                Vector3Int pos = new Vector3Int(gridPosition.x + x, gridPosition.y, gridPosition.z + y);
                if (placedObjects.ContainsKey(pos))
                {
                    return false; // sudah terisi
                }
            }
        }
        return true; // semua kosong
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (placedObjects.TryGetValue(gridPosition, out var data))
        {
            foreach (var pos in data.occupiedPositions)
            {
                placedObjects.Remove(pos);
                Debug.Log("[GridData] Menghapus posisi: " + pos);
            }

            Debug.Log("[GridData] Objek dihapus sepenuhnya dari grid.");
        }
        else
        {
            Debug.Log("[GridData] Tidak ada data di posisi: " + gridPosition + " untuk dihapus.");
        }
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
