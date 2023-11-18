using Common;
using GameCore.Character.Animation;
using GameCore.Common.Messages;
using GameCore.InteractiveObjects;
using LocalMessages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacuationInteractive : BaseTriggerObject
{

    private void Start()
    {
        var _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
        _messageBroker.Subscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
        gameObject.SetActive(false);
    }

    private void OnEvacuationActivated(ref ActivateEvacuationMessage value)
    {
        gameObject.SetActive(value.active);
    }

    protected override void OnPlayerEnter()
    {
        var message = new PlayerEvacuatedMessage();
        var messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
        messageBroker.Trigger(ref message);

    }

    protected override void OnPlayerStay()
    {
    }

    protected override void OnPlayerExit()
    {
    }

    private void OnDestroy()
    {
        var _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
        _messageBroker.Unsubscribe<ActivateEvacuationMessage>(OnEvacuationActivated);
    }

}