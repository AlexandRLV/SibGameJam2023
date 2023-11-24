using System.Collections;
using Common;
using Common.DI;
using GameCore.Common;
using GameCore.RoundMissions;
using UnityEngine;

namespace Startup.GameplayInitializers
{
    public class RoundInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var roundControllerPrefab = Resources.Load<RoundController>("Round/RoundController");
            var roundController = GameContainer.InstantiateAndResolve(roundControllerPrefab);
            GameContainer.InGame.Register(roundController);

            var missionsControllerPrefab = Resources.Load<MissionsController>("Prefabs/MissionsController");
            var missionsController = GameContainer.InstantiateAndResolve(missionsControllerPrefab);
            GameContainer.InGame.Register(missionsController);
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}