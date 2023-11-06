using GameCore.Character.Animation;
using GameCore.InteractiveObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacuationInteractive : InteractiveObject
{
    public override AnimationType InteractAnimation => AnimationType.Eat;

    public override void Interact()
    {

    }
}
