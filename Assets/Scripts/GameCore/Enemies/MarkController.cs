using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkController : MonoBehaviour
{
    [SerializeField] GameObject questionMark;
    [SerializeField] GameObject exclamationMark;
    bool isActiveAnything;

    private void Start()
    {
        questionMark.SetActive(false);
        exclamationMark.SetActive(false);
        isActiveAnything = false;
    }

    public void LookAt(Transform target)
    {
        if (isActiveAnything) transform.LookAt(target);
    }

    public void SetQuestionMark()
    {
        questionMark.SetActive(true);
        exclamationMark.SetActive(false);
        isActiveAnything = true;
    }

    public void SetExclamationMark()
    {
        questionMark.SetActive(false);
        exclamationMark.SetActive(true);
        Debug.Log("hello");
        isActiveAnything = true;
    }

    public void ResetMarks()
    {
        questionMark.SetActive(false);
        exclamationMark.SetActive(false);
        isActiveAnything = false;
    }
}
