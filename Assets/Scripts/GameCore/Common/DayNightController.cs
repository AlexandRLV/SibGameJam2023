using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [SerializeField] Transform dayLight;
    [SerializeField] Transform moonLight;
    [SerializeField] float dayDuration;
    [SerializeField] float startRotX = -90;
    [SerializeField] float finalRotX = 270;
    [SerializeField] bool testMode = false;

    float currentRotX;

    private void Start()
    {
        if(!testMode) transform.rotation = Quaternion.Euler(startRotX, 0, 0);
        currentRotX = startRotX;
    }

    private void Update()
    {
        if (currentRotX < finalRotX && !testMode)
        {
            currentRotX += 360 / dayDuration * Time.deltaTime;
            transform.rotation = Quaternion.Euler(currentRotX, 0, 0);


        }
    }
}
