using System.Collections;
using Common;
using GameCore.Common;
using GameCore.RoundMissions;
using UnityEngine;

namespace Startup.GameplayInitializers
{
    public class SinglePlayerRoundInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var roundControllerPrefab = Resources.Load<RoundController>("Round/RoundController");
            var roundController = Object.Instantiate(roundControllerPrefab);
            GameContainer.InGame.Register(roundController);

            var missionsControllerPrefab = Resources.Load<MissionsController>("Prefabs/MissionsController");
            var missionsController = Object.Instantiate(missionsControllerPrefab);
            missionsController.Initialize();
            GameContainer.InGame.Register(missionsController);
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}