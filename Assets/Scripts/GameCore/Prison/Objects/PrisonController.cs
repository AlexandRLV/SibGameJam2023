using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonController : MonoBehaviour
{
    [SerializeField] float openSpeed = 10;
    [SerializeField] float timeToOpen = 1;
    [SerializeField] Vector3 openingDirection = Vector3.down;
    [SerializeField] Transform door;
    float openingTime;
    bool isOpened = false;

    [SerializeField] PrisonMouseController[] mouseControllers;

    private void Awake()
    {
        mouseControllers = GetComponentsInChildren<PrisonMouseController>();
    }

    public void OpenDoor()
    {
        if (door == null || mouseControllers.Length == 0) return;
        if (!isOpened) StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        isOpened = true;
        while (openingTime < timeToOpen)
        {
            openingTime += Time.deltaTime;
            door.transform.Translate(openingDirection * openSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        foreach (PrisonMouseController controller in mouseControllers)
        {
            controller.isReleased = true;
        }
    }
}
