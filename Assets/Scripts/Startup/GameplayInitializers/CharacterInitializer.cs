using System.Collections;
using Common;
using GameCore;
using UnityEngine;

namespace Startup.GameplayInitializers
{
    public class CharacterInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var spawns = GameContainer.InGame.Resolve<PlayerSpawns>();
            
            var characterPrefab = Resources.Load<GameObject>("Character/Character");
            var character = Object.Instantiate(characterPrefab);
            character.transform.SetPositionAndRotation(spawns.SpawnPoint.position, spawns.SpawnPoint.rotation);
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}