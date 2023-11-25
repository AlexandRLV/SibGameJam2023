using System.Collections.Generic;
using GameCore.Enemies.EnemyObject;
using UnityEngine;

namespace GameCore.Enemies.RouteControl
{
    public class Route : MonoBehaviour
    {
        [SerializeField] List<Waypoint> waypoints;
        [SerializeField] EnemyController enemyPrefab;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                waypoints.Add(transform.GetChild(i).GetComponent<Waypoint>());
            }

            EnemyController newEnemy = Instantiate(enemyPrefab, waypoints[0].transform.position, waypoints[0].transform.rotation);
            newEnemy.Init(waypoints);
        }
    }
}
