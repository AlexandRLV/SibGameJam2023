﻿using System.Collections.Generic;
using Common.DI;
using GameCore.Camera;
using GameCore.Character.Movement;
using UnityEngine;

namespace GameCore.Player
{
    public class OneMouseSinglePlayer : MonoBehaviour, IPlayer
    {
        public PlayerMouseType MouseType { get; private set; }
        public CharacterMovement CurrentMovement { get; private set; }
        public CharacterMovement LastMovement { get; private set; }
        public List<int> MovementObjects { get; private set; }

        [Inject] private GameCamera _gameCamera;

        public void Initialize(CharacterMovement movement, PlayerMouseType mouseType)
        {
            LastMovement = movement;
            CurrentMovement = movement;
            CurrentMovement.Posess();

            MovementObjects = new List<int> { movement.gameObject.GetInstanceID() };
            MouseType = mouseType;
            
            _gameCamera.SetTarget(movement.transform);
        }
        
        public void Unposess()
        {
            if (CurrentMovement != null)
            {
                LastMovement = CurrentMovement;
                CurrentMovement.Unposess();
            }
            
            CurrentMovement = null;
        }
    }
}