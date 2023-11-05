using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacuationPointController : MonoBehaviour
{
    [SerializeField] Transform evacuationPoint;

    public Transform EvacuationPoint => evacuationPoint;

    public static EvacuationPointController Instance;

    private void Awake()
    {
        Instance = this;
        evacuationPoint = transform;
    }
}
