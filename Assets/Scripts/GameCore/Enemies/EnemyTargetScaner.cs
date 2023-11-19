using Common;
using GameCore.Player;
using System.Collections;
using System.Collections.Generic;
using GameCore.Character.Movement;
using LocalMessages;
using UnityEngine;
using UnityEditor;

public class EnemyTargetScaner : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField] bool showGizmos;
    [SerializeField] Transform eyeCenter;

    [SerializeField] float alertRadius = 8f;
    [SerializeField] float viewDistance = 10f;
    [SerializeField, Range(0f, 360f)] float viewAngle = 360f;
    [SerializeField] float heightOffset = 1f;
    [SerializeField] float maxHeightDifference = 1f;

    [SerializeField] LayerMask targetLayer, obstacleLayer;

    #endregion

    #region Private Variables

    [SerializeField]List<Transform> targetList = new List<Transform>();
    Transform customTarget;
    Vector3 eyePos;
    RaycastHit hit;

    #endregion

    #region Properties

    public float ViewAngle => viewAngle;
    public float ViewDistance => viewDistance;
    public LayerMask ObstacleLayer => obstacleLayer;

    #endregion

    #region Private Methods

    private void RunScanner()
    {
        if (!eyeCenter)
        {
            Debug.LogError("Center of Scanner not assigned");
            return;
        }

        eyePos = eyeCenter.position;

        FindVisibleTarget();
        RemoveTargetFromList();
        AddCustomtargetToList();
    }

    private void FindVisibleTarget()
    {
        if (GameContainer.InGame == null) return;
        var player = GameContainer.InGame.Resolve<GamePlayer>();
        if (player == null || player.CurrentCharacter == null) return;

        var collider = player.CurrentCharacter.Collider;
        // Detect without obstacles
        Vector3 targetSize = collider.bounds.size;
        Transform target = collider.transform;

        Vector3 toPlayer = target.transform.position - eyePos;

        if (Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
        {
            return;
        }

        if (Vector3.Distance(transform.position, target.position) < alertRadius)
        {
            if (!targetList.Contains(target))
            {
                targetList.Add(target);
            }
            
            return;
        }

        Vector3 dirToTarget = (target.position - transform.position).normalized;

        // Detect include obstacles
        // Detect if any Obstacle come in path
        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
        {
            targetSize.y -= 0.05f; // Manual offset in size so that raycast won't go above the mesh

            float offsetX = targetSize.x / 2;
            float offsetY = targetSize.y / 2;

            int rayCastIteration = 0;

            for (int j = 0; j < 3; j++) // Row of RayCast
            {
                for (int k = 0; k < 5; k++) // Column of RayCast
                {
                    Vector3 targetPosition = target.position + new Vector3(offsetX, offsetY, 0);

                    float distToTarget = Vector3.Distance(transform.position, target.position);

                    dirToTarget = (targetPosition - transform.position).normalized;

                    if (!Physics.Raycast(transform.position, dirToTarget, out hit, distToTarget, obstacleLayer))
                    {

#if UNITY_EDITOR
                        if (showGizmos)
                            Debug.DrawLine(transform.position, targetPosition, Color.green); // Debug RayCast
#endif

                        if (!targetList.Contains(target))
                        {
                            targetList.Add(target);
                        }

                        return; // Target is detected no need to go further, so jump out of the Main loop
                    }
#if UNITY_EDITOR
                    if (showGizmos)
                        Debug.DrawLine(transform.position, targetPosition, Color.red); // Debug RayCast
#endif

                    offsetY -= targetSize.y / 4;
                }

                rayCastIteration++;
                offsetY = targetSize.y / 2;
                offsetX -= targetSize.x / 2;

            }

            if (rayCastIteration >= 3 && targetList.Contains(target))
            {
                targetList.Remove(target);
            }
        }
    }

    private void RemoveTargetFromList()
    {
        if (targetList.Count == 0) return;

        for (int i = 0; i < targetList.Count; i++)
        {
            Transform target = targetList[i];

            //Null Check-
            if (target == null)
            {
                targetList.RemoveAt(i);
            }

            //not active in hierarchy
            if (!target.gameObject.activeInHierarchy)
            {
                targetList.Remove(target);
                continue;
            }

            //Out of View Radius
            if (Vector3.Distance(transform.position, target.position) > viewDistance)
            {
                targetList.Remove(target);
                continue;
            }

            //Inside Alert Radius
            if (Vector3.Distance(transform.position, target.position) < alertRadius) continue;

            //HeightCheck
            Vector3 toPlayer = target.transform.position - eyePos;
            if (Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
            {
                targetList.Remove(target);
            }

            //Out of FOV
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) > viewAngle / 2)
            {
                targetList.Remove(target);
            }
        }
    }

    private void AddCustomtargetToList()
    {
        if (customTarget != null)
        {
            targetList.Add(customTarget);
            customTarget = null;
        }
    }
    #endregion

    #region Public Methods

    // return the nearest target to enemy
    // method should call from enemy update/fixed update
    public Transform GetNearestTarget()
    {
        RunScanner();

        if (targetList.Count == 0) return null;

        Transform _nearestTarget = targetList[0];

        for (int i = 0; i < targetList.Count; i++)
        {
            if (Vector3.Distance(transform.position, targetList[i].position) <
                Vector3.Distance(transform.position, _nearestTarget.position))
            {
                _nearestTarget = targetList[i];
            }

        }
        return _nearestTarget;
    }

    public List<Transform> GetTargetList()
    {
        RunScanner();

        if (targetList.Count == 0) return null;

        return targetList;
    }

    #endregion

    /*
    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || transform == null) return;

        Gizmos.color = Color.red;

        if (hit.collider != null) Gizmos.DrawSphere(hit.point, 0.15f);

        //Eye Location
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * heightOffset, 0.2f);

        //Height Check Area
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position + Vector3.up * maxHeightDifference, Vector3.up, viewDistance);
        UnityEditor.Handles.DrawWireDisc(transform.position + Vector3.down * maxHeightDifference, Vector3.up, viewDistance);

        //Always Detect Radius
        Color r = new Color(0.5f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        //View Radius
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, viewDistance);

        //View Angle
        Color b = new Color(0, 0.5f, 0.7f, 0.2f);
        UnityEditor.Handles.color = b;
        Vector3 rotatedForward = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotatedForward, viewAngle, viewDistance);

        //To Target Line
        Gizmos.color = Color.red;
        foreach (Transform t in targetList)
        {
            Gizmos.DrawLine(transform.position, t.position);
            Gizmos.DrawCube(t.position, new Vector3(0.3f, 0.3f, 0.3f));
        }
    }
    */
}