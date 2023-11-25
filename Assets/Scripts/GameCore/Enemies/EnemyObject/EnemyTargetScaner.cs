using System.Collections.Generic;
using Common.DI;
using GameCore.Player;
using UnityEngine;

namespace GameCore.Enemies.EnemyObject
{
    public class EnemyTargetScaner : MonoBehaviour
    {
#region Properties
        public float ViewAngle => viewAngle;
        public float ViewDistance => viewDistance;
        public LayerMask ObstacleLayer => obstacleLayer;
#endregion
        
#region Serialized Variables
        [SerializeField] bool showGizmos;
        [SerializeField] Transform eyeCenter;

        [SerializeField] float alertRadius = 8f;
        [SerializeField] float viewDistance = 10f;
        [SerializeField, Range(0f, 360f)] float viewAngle = 360f;
        [SerializeField] float heightOffset = 1f;
        [SerializeField] float maxHeightDifference = 1f;

        [SerializeField] LayerMask targetLayer, obstacleLayer;
        [SerializeField] List<Transform> targetList;
#endregion

#region Private Variables
        private Transform _customTarget;
        private Vector3 _eyePos;
        private RaycastHit _hit;
#endregion

#region Private Methods
        private void RunScanner()
        {
            if (!eyeCenter)
                return;

            _eyePos = eyeCenter.position;

            FindVisibleTarget();
            RemoveTargetFromList();
            AddCustomtargetToList();
        }

        private void FindVisibleTarget()
        {
            if (GameContainer.InGame == null) return;
            var player = GameContainer.InGame.Resolve<IPlayer>();
            
            if (player == null || player.CurrentMovement == null) return;

            var collider = player.CurrentMovement.Collider;
            // Detect without obstacles
            var targetSize = collider.bounds.size;
            var target = collider.transform;

            var toPlayer = target.transform.position - _eyePos;

            if (Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
                return;

            if (Vector3.Distance(transform.position, target.position) < alertRadius)
            {
                if (!targetList.Contains(target))
                    targetList.Add(target);
            
                return;
            }

            var dirToTarget = (target.position - transform.position).normalized;

            // Detect include obstacles
            // Detect if any Obstacle come in path
            if (Vector3.Angle(transform.forward, dirToTarget) > viewAngle / 2)
                return;
            
            targetSize.y -= 0.05f; // Manual offset in size so that raycast won't go above the mesh

            float offsetX = targetSize.x / 2;
            float offsetY = targetSize.y / 2;

            int rayCastIteration = 0;

            for (int j = 0; j < 3; j++) // Row of RayCast
            {
                for (int k = 0; k < 5; k++) // Column of RayCast
                {
                    var targetPosition = target.position + new Vector3(offsetX, offsetY, 0);
                    float distToTarget = Vector3.Distance(transform.position, target.position);
                    dirToTarget = (targetPosition - transform.position).normalized;

                    if (!Physics.Raycast(transform.position, dirToTarget, out _hit, distToTarget, obstacleLayer))
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
                targetList.Remove(target);
        }

        private void RemoveTargetFromList()
        {
            if (targetList.Count == 0) return;

            for (int i = targetList.Count - 1; i >= 0; i--)
            {
                var target = targetList[i];

                //Null Check-
                if (target == null)
                {
                    targetList.RemoveAt(i);
                    continue;
                }

                //not active in hierarchy
                if (!target.gameObject.activeInHierarchy)
                {
                    targetList.RemoveAt(i);
                    continue;
                }

                //Out of View Radius
                if (Vector3.Distance(transform.position, target.position) > viewDistance)
                {
                    targetList.RemoveAt(i);
                    continue;
                }

                //Inside Alert Radius
                if (Vector3.Distance(transform.position, target.position) < alertRadius)
                    continue;

                //HeightCheck
                var toPlayer = target.transform.position - _eyePos;
                if (Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
                {
                    targetList.RemoveAt(i);
                    continue;
                }

                //Out of FOV
                var dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) > viewAngle / 2)
                    targetList.RemoveAt(i);
            }
        }

        private void AddCustomtargetToList()
        {
            if (_customTarget == null)
                return;
            
            targetList.Add(_customTarget);
            _customTarget = null;
        }
#endregion

#region Public Methods
        // return the nearest target to enemy
        // method should call from enemy update/fixed update
        public Transform GetNearestTarget()
        {
            RunScanner();

            if (targetList.Count == 0) return null;

            var nearestTarget = targetList[0];
            foreach (var target in targetList)
            {
                if (Vector3.Distance(transform.position, target.position) <
                    Vector3.Distance(transform.position, nearestTarget.position))
                {
                    nearestTarget = target;
                }
            }
            
            return nearestTarget;
        }
#endregion
    }
}