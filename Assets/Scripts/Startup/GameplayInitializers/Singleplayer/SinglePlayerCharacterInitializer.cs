using System.Collections;
using Common;
using Common.DI;
using GameCore;
using GameCore.Character.Animation;
using GameCore.Character.Movement;
using GameCore.Player;
using UnityEngine;

namespace Startup.GameplayInitializers
{
    public class SinglePlayerCharacterInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {
            var spawns = GameContainer.InGame.Resolve<PlayerSpawns>();
            
            var thinMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/ThinMouseVisuals");
            var thinMouseVisuals = Object.Instantiate(thinMouseVisualsPrefab);

            var fatMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/FatMouseVisuals");
            var fatMouseVisuals = Object.Instantiate(fatMouseVisualsPrefab);

            var thinMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/ThinMouseCharacter");
            var thinMouseMovement = Object.Instantiate(thinMousePrefab);
            thinMouseMovement.transform.SetPositionAndRotation(spawns.SpawnPoints[0].position, spawns.SpawnPoints[0].rotation);
            thinMouseMovement.Initialize(thinMouseVisuals);

            var fatMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/FatMouseCharacter");
            var fatMouseMovement = Object.Instantiate(fatMousePrefab);
            fatMouseMovement.transform.SetPositionAndRotation(spawns.SpawnPoints[1].position, spawns.SpawnPoints[1].rotation);
            fatMouseMovement.Initialize(fatMouseVisuals);

            var playerPrefab = Resources.Load<TwoMousePlayer>("Prefabs/TwoMousePlayer");
            var player = Object.Instantiate(playerPrefab);
            player.Initialize(fatMouseMovement, thinMouseMovement);

            GameContainer.InGame.Register<IPlayer>(player);

            yield return null;
        }

        public void Dispose()
        {
        }
    }
}