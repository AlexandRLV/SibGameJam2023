﻿using System.Collections;
using Common.DI;
using GameCore;
using GameCore.Character.Animation;
using GameCore.Character.Movement;
using GameCore.Player;
using UnityEngine;

namespace Startup.GameplayInitializers.Tutorial
{
    public class TutorialCharacterInitializer : IInitializer
    {
        public IEnumerator Initialize()
        {var spawns = GameContainer.InGame.Resolve<PlayerSpawns>();
            
            var thinMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/ThinMouseVisuals");
            var thinMouseVisuals = Object.Instantiate(thinMouseVisualsPrefab);

            var fatMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/FatMouseVisuals");
            var fatMouseVisuals = Object.Instantiate(fatMouseVisualsPrefab);

            var thinMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/ThinMouseCharacter");
            var thinMouseMovement = GameContainer.InstantiateAndResolve(thinMousePrefab);
            thinMouseMovement.transform.SetPositionAndRotation(spawns.ThinSpawn.position, spawns.ThinSpawn.rotation);
            thinMouseMovement.Initialize(thinMouseVisuals);

            var fatMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/FatMouseCharacter");
            var fatMouseMovement = GameContainer.InstantiateAndResolve(fatMousePrefab);
            fatMouseMovement.transform.SetPositionAndRotation(spawns.FatSpawn.position, spawns.FatSpawn.rotation);
            fatMouseMovement.Initialize(fatMouseVisuals);

            Physics.IgnoreCollision(thinMouseMovement.Collider, fatMouseMovement.Collider);

            var playerPrefab = Resources.Load<TwoMousePlayer>("Prefabs/TwoMousePlayer");
            var player = GameContainer.InstantiateAndResolve(playerPrefab);
            player.Initialize(fatMouseMovement, thinMouseMovement, false);

            GameContainer.InGame.Register<IPlayer>(player);
            yield return null;
        }

        public void Dispose()
        {
        }
    }
}