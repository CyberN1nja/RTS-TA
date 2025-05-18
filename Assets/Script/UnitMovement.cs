using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    private Camera cam;
    private NavMeshAgent agent;
    public LayerMask ground;

    public bool isCommandedToMove;

    private DirectionIndicator directionIndicator;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        directionIndicator = GetComponent<DirectionIndicator>();
    }

    private void Update()
    {
        // Deteksi klik kanan untuk gerakkan unit
        if (Input.GetMouseButtonDown(1) && IsMovingPossible())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                MoveTo(hit.point, true);

                SoundManager.Instance.PlayUnitCommandSound();

                directionIndicator.DrawLine(hit);
            }
        }

        // Optional: deteksi jika sudah sampai tujuan
        // if (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        // {
        //     isCommandedToMove = false;
        // }
    }

    public void MoveTo(Vector3 position, bool isPlayerCommand)
    {
        isCommandedToMove = true;
        StartCoroutine(NoCommand());

        // ✅ Cek apakah agent ada di atas NavMesh sebelum memanggil SetDestination
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(position);
        }
        else
        {
            Debug.LogWarning("[UnitMovement] Agent belum berada di atas NavMesh! Posisi sekarang: " + transform.position);
            Debug.DrawRay(transform.position, Vector3.down * 2f, Color.red, 2f); // debug visual
            return;
        }

        // ✅ Batalkan panen jika ini perintah manual
        if (isPlayerCommand)
        {
            Harvester harvester = GetComponent<Harvester>();
            if (harvester != null)
            {
                if (harvester.assignedNode == null ||
                    Vector3.Distance(harvester.assignedNode.position, position) > 1f)
                {
                    harvester.CancelHarvesting();
                }
            }
        }
    }

    private bool IsMovingPossible()
    {
        return CursorManager.Instance.currentCursor != CursorManager.CursorType.UnAvailable;
    }

    private IEnumerator NoCommand()
    {
        yield return new WaitForSeconds(1);
        isCommandedToMove = false;
    }
}
