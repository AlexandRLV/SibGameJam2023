using System.Collections;
using System.Collections.Generic;
using Common.DI;
using GameCore.Common;
using GameCore.RoundControl;
using GameCore.RoundMissions;
using GameCore.RoundMissions.Missions;
using UnityEngine;

namespace Startup.GameplayInitializers.Common
{
    public class RoundInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var roundSettings = Resources.Load<RoundSettings>("Round/Round Settings");
            GameContainer.InGame.Register(roundSettings);

            var roundControllerPrefab = Resources.Load<RoundController>("Round/RoundController");
            var roundController = GameContainer.InstantiateAndResolve(roundControllerPrefab);
            GameContainer.InGame.Register(roundController);

            var missionsData = Resources.Load<MissionsData>("Missions/Missions Data");
            GameContainer.InGame.Register(missionsData);

            var missionsControllerPrefab = Resources.Load<MissionsController>("Prefabs/MissionsController");
            var missionsController = GameContainer.InstantiateAndResolve(missionsControllerPrefab);
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