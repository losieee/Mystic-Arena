using Fusion;
using System.Collections;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector3 direction;
    public Vector3 lookDirection;
}

public class NetworkInputHandler : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float horizontal;
    public float vertical;
    public float interactionRange;

    [Header("Interaction Settings")]
    public Transform spot;
    public bool hasHealed = false;

    [Header("Health")]
    private PlayerStats holly_Knight_Stats;

    private Camera mainCamera;
    private KnightMove knightMove;

    private void Start()
    {
        mainCamera = Camera.main;
        knightMove = GetComponent<KnightMove>();
    }

    public NetworkInputData GetNetworkInput()
    {
        var data = new NetworkInputData();

        if (Input.GetMouseButtonDown(1))
        {
            // Handle right-click interaction
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("NPC") && !hasHealed &&
                    Vector3.Distance(transform.position, hit.point) <= interactionRange)
                {
                    knightMove.HealPlayer();
                    spot.gameObject.SetActive(false);
                    return data;
                }

                // Update spot position and interaction
                spot.position = hit.point;
                spot.transform.localScale = Vector3.one * 1.5f;
                spot.gameObject.SetActive(true);
                StartCoroutine(ShrinkSpot());
                knightMove.SetDestination(hit.point);
            }
        }

        // Handle movement inputs
        if (mainCamera != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            data.direction = cameraRight * horizontal + cameraForward * vertical;
        }

        if (data.direction.magnitude > 1)
        {
            data.direction.Normalize();
        }

        data.lookDirection = mainCamera != null ? mainCamera.transform.forward : Vector3.forward;

        return data;
    }

    private IEnumerator ShrinkSpot()
    {
        float shrinkAmount = 0.3f;
        float duration = 0.1f;

        while (spot.transform.localScale.x > 0.1f)
        {
            spot.transform.localScale -= Vector3.one * shrinkAmount;
            yield return new WaitForSeconds(duration);
        }

        spot.gameObject.SetActive(false);
    }
}
