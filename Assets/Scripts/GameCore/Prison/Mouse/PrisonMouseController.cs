using UnityEngine;

public class PrisonMouseController : MonoBehaviour
{
    public bool isReleased = false;
    private PrisonMouseMovement movement;

    private void Start()
    {
        movement = GetComponent<PrisonMouseMovement>();
        movement.Init();
    }

    private void FixedUpdate()
    {
        if (isReleased == false)
        {
            movement.PrisonMovement();
        }
        else
        {
            movement.EvacuationMovement();
        }
    }

}
