using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRemover : MonoBehaviour
{
    public GridData gridData;
    public Vector3Int gridPosition;
    public Vector2Int size;

    public void RemoveFromGrid()
    {
        if (gridData != null)
        {
            gridData.RemoveObjectAt(gridPosition);
            Debug.Log("[GridRemover] Grid dihapus untuk posisi: " + gridPosition);
        }
    }

    private void OnDestroy()
    {
        RemoveFromGrid();
    }
}
