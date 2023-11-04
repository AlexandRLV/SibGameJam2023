using System.Collections;
using System.Collections.Generic;
using Common;
using GameCore.InteractiveObjects;
using UnityEngine;

namespace Startup.GameplayInitializers
{
    public class InteractiveObjectsInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            InteractiveObject eatPrefab = Resources.Load<EatableObject>("Prefabs/InteractiveObjects/Eat");
            InteractiveObject trapPrefab = Resources.Load<TrapObject>("Prefabs/InteractiveObjects/Trap");
            var spawnPoints = GameContainer.InGame.Resolve<InteractiveObjectsSpawns>();

            int numberOfObjects = spawnPoints.spawnPoints.Count;

            for (int i = 0; i < numberOfObjects; i++)
            {
                var nextObject = i % 2 == 0 ? eatPrefab : trapPrefab;
                var spawnedObject = Object.Instantiate(nextObject);
                int randomNumber = Random.Range(0, spawnPoints.spawnPoints.Count);
                spawnedObject.transform.position =
                    spawnPoints.spawnPoints[randomNumber].position;
                spawnPoints.spawnPoints.RemoveAt(randomNumber);
            }
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}