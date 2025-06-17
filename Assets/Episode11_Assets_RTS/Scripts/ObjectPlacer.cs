using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        Debug.Log($"[ObjectPlacer] Menempatkan objek {prefab.name} di {position}");

        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;

        Constructable constructable = newObject.GetComponent<Constructable>();
        if (constructable != null)
        {
            constructable.ConstructableWasPlaced(position);
        }

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }


    internal GameObject PlaceObjectReturnGO(GameObject prefab, Vector3 vector3)
    {
        throw new NotImplementedException();
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObjects.Count <= gameObjectIndex 
            || placedGameObjects[gameObjectIndex] == null)
             return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
