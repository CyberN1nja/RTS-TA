using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitList = new List<GameObject>();
    public List<GameObject> unitSelected = new List<GameObject>();

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask attackable;

    public bool attackCursorVisible;
    
    public LayerMask constructable;

    public GameObject groundMarker;

    private Camera cam;

    public bool playedDuringThisDrag = false;

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

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        CleanDestroyedUnits(); // Bersihkan unit yang sudah null/destroy

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelect(hit.collider.gameObject);
                }
                else
                {
                    SelectByClicking(hit.collider.gameObject);
                }
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    DeselectAll();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && unitSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.transform.position = hit.point;
                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }

        if (unitSelected.Count > 0 && AtleastOneOffensiveUnit(unitSelected))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable))
            {
                Debug.Log("Enemy Hovered with mouse");
                attackCursorVisible = true;

                if (Input.GetMouseButtonDown(1))
                {
                    Transform target = hit.transform;

                    foreach (GameObject unit in unitSelected)
                    {
                        if (unit != null && unit.GetComponent<AttackController>())
                        {
                            unit.GetComponent<AttackController>().targetToAttack = target;
                        }
                    }
                }
            }
            else
            {
                attackCursorVisible = false;
            }
        }

        if (unitSelected.Count > 0 && OnlyHarvestersSelected())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                ResourceNode resourceNode = hit.transform.GetComponent<ResourceNode>();
                if (resourceNode != null)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        Transform node = hit.transform;

                        foreach (GameObject unit in unitSelected)
                        {
                            Harvester harvester = unit.GetComponent<Harvester>();
                            if (harvester != null)
                            {
                                harvester.assignedNode = node;
                            }
                        }
                    }
                }

                
            }
        } 

        CursorSelector();
    }

    private bool OnlyHarvestersSelected()
    {
        if (unitSelected.Count == 0) return false;

        foreach (GameObject unit in unitSelected)
        {
            if (unit == null || unit.GetComponent<Harvester>() == null)
            {
                return false;
            }
        }
        return true;
    }


    private void CursorSelector()
    {
        if (TrySetSelectTableCursor()) return;
        if (TrySetSellCursor()) return;
        if (TrySetAttackCursor()) return;
        if (TrySetUnAvailableCursor()) return;
        if (TrySetGatheringCursor()) return;
        if (TrySetWalkableCursor()) return;

        CursorManager.Instance.SetMarkerType(CursorManager.CursorType.None);
    }

    private bool RayHits(LayerMask mask, out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
    }

    private bool TrySetWalkableCursor()
    {
        if (unitSelected.Count > 0 && RayHits(ground, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Walkable);
            return true;
        }
        return false;
    }

    private bool TrySetGatheringCursor()
    {
        if (unitSelected.Count > 0 && OnlyHarvestersSelected())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 
                out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.GetComponent<ResourceNode>() != null)
                {
                    CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Gathering);
                    return true;
                }
            }
        }

        return false;
    }

    private bool TrySetUnAvailableCursor()
    {
        if (unitSelected.Count > 0 && RayHits(constructable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.UnAvailable);
            return true;
        }
        return false;
    }

    private bool TrySetAttackCursor()
    {
        if (unitSelected.Count > 0 && AtleastOneOffensiveUnit(unitSelected) && RayHits(attackable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Attackable);
            return true;
        }
        return false;
    }

    private bool TrySetSellCursor()
    {
        if (ResourceManager.Instance.placementSystem.inSellMode)
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.SellCursor);
            return true;
        }
        return false;
    }

    private bool TrySetSelectTableCursor()
    {
        if (RayHits(clickable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Selectable);
            return true;
        }
        return false;
    }

    private bool AtleastOneOffensiveUnit(List<GameObject> unitSelected)
    {
        foreach (GameObject unit in unitSelected)
        {
            if (unit != null && unit.GetComponent<AttackController>())
            {
                return true;
            }
        }
        return false;
    }

    private void MultiSelect(GameObject unit)
    {
        if (!unitSelected.Contains(unit))
        {
            unitSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
            unitSelected.Remove(unit);
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in unitSelected)
        {
            if (unit != null)
            {
                SelectUnit(unit, false);
            }
        }

        groundMarker.SetActive(false);
        unitSelected.Clear();
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();
        unitSelected.Add(unit);
        SelectUnit(unit, true);
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        if (unit != null && unit.GetComponent<UnitMovement>())
        {
            unit.GetComponent<UnitMovement>().enabled = shouldMove;
        }
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        GameObject indicator = unit.transform.Find("Indicator").gameObject;

        if (indicator.activeInHierarchy && !playedDuringThisDrag)
        {
            SoundManager.Instance.PlayUnitSelectionSound();
            playedDuringThisDrag = true;
        }

        indicator.SetActive(isVisible);
    }

    internal void DragSelect(GameObject unit)
    {
        if (!unitSelected.Contains(unit))
        {
            unitSelected.Add(unit);
            SelectUnit(unit, true);
        }
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {

        TriggerSelectionIndicator(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }

    private void CleanDestroyedUnits()
    {
        unitSelected.RemoveAll(unit => unit == null);
    }
}
