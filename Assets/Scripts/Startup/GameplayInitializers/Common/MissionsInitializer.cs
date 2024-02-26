﻿using System.Collections.Generic;
using Common.DI;
using GameCore.RoundMissions;
using GameCore.RoundMissions.Missions;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class MissionsInitializer : InitializerBase
    {
        [SerializeField] private MissionsData _missionsData;
        
        public override void Initialize()
        {
            GameContainer.InGame.Register(_missionsData);
            
            var missionsController = GameContainer.CreateGameObjectWithComponent<MissionsController>("MissionsController");
            GameContainer.InGame.Register(missionsController);
            
            var missions = new List<MissionBase>
            {
                GameContainer.Create<SaveAgentsMission>(),
                GameContainer.Create<FindCactusMission>(),
                GameContainer.Create<EvacuateMission>(),
            };
            missionsController.Initialize(missions);
        }

        public override void Dispose()
        {
        }
    }
}