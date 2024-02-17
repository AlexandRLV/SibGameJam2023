﻿using System.Collections.Generic;
using Common.DI;
using GameCore.Common;
using GameCore.RoundControl;
using GameCore.RoundMissions;
using GameCore.RoundMissions.Missions;
using UnityEngine;

namespace Startup.GameplayInitializers.Tutorial
{
    public class TutorialRoundInitializer : InitializerBase
    {
        [SerializeField] private RoundSettings _roundSettings;
        [SerializeField] private MissionsData _missionsData;
        
        public override void Initialize()
        {
            GameContainer.InGame.Register(_roundSettings);

            var roundController = GameContainer.CreateGameObjectWithComponent<RoundController>("RoundController");
            GameContainer.InGame.Register(roundController);
            
            GameContainer.InGame.Register(_missionsData);
            
            var missionsController = GameContainer.CreateGameObjectWithComponent<MissionsController>("MissionsController");
            GameContainer.InGame.Register(missionsController);
            
            var missions = new List<MissionBase>
            {
                GameContainer.Create<SaveAgentsMission>(),
                GameContainer.Create<EvacuateMission>(),
            };
            missionsController.Initialize(missions);
        }

        public override void Dispose()
        {
        }
    }
}