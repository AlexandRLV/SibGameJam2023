using Common;
using GameCore.Character.Animation;
using GameCore.Common;
using GameCore.Common.Messages;
using GameCore.InteractiveObjects;
using LocalMessages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusInteractiveObject : InteractiveObject
{
    public override AnimationType InteractAnimation => AnimationType.Eat;
    [SerializeField] Collider mainCollider;
    [SerializeField]Vector3 a, b;
    Vector3 startScale;
    Vector3 endScale;
    bool canStart = false;
    bool isFinished = false;

    public override void Interact()
    {
        var roundController = GameContainer.InGame.Resolve<RoundController>();
        roundController.CatchCactus();
        startScale = transform.localScale;
        endScale = Vector3.zero;
        a = transform.position;
        canStart = true;
        mainCollider.isTrigger = true;
    }

    private void Update()
    {
        if (canStart == true && isFinished == false)
        {
            Debug.Log("hello");
            a = transform.position;
            b = Movement.gameObject.transform.position;
            transform.position = Vector3.Lerp(a, b, Time.deltaTime * 2f);
            transform.localScale = Vector3.Lerp(startScale, endScale, Time.deltaTime * 20f);

            if (Vector3.Distance(a,b) < 0.5f)
            {
                Debug.Log("isFinished");
                isFinished = true;
                gameObject.SetActive(false);
                return;
            }
        }

    }
}
