using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaypoints : MonoBehaviour
{
    [SerializeField]private List<Transform> waypoints;
    public List<Transform> Waypoints => waypoints;

    private void Awake()
    {
        
        
    }
}
