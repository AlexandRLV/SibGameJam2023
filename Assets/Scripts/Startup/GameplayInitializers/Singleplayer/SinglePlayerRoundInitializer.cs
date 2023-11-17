using System.Collections;
using Common;
using GameCore.Common;
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
            
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}