using Common.DI;
using GameCore;
using GameCore.Character.Animation;
using GameCore.Character.Movement;
using GameCore.Player;
using UnityEngine;

namespace Startup.GameplayInitializers.Singleplayer
{
    public class SinglePlayerCharacterInitializer : InitializerBase
    {
        public override void Initialize()
        {
            var spawns = GameContainer.InGame.Resolve<PlayerSpawns>();
            
            var thinMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/ThinMouseVisuals");
            var thinMouseVisuals = Instantiate(thinMouseVisualsPrefab);

            var fatMouseVisualsPrefab = Resources.Load<CharacterVisuals>("Prefabs/CharacterVisuals/FatMouseVisuals");
            var fatMouseVisuals = Instantiate(fatMouseVisualsPrefab);

            var thinMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/ThinMouseCharacter");
            var thinMouseMovement = GameContainer.InstantiateAndResolve(thinMousePrefab);
            thinMouseMovement.transform.SetPositionAndRotation(spawns.ThinSpawn.position, spawns.ThinSpawn.rotation);
            thinMouseMovement.Initialize(thinMouseVisuals);

            var fatMousePrefab = Resources.Load<CharacterMovement>("Prefabs/Characters/FatMouseCharacter");
            var fatMouseMovement = GameContainer.InstantiateAndResolve(fatMousePrefab);
            fatMouseMovement.transform.SetPositionAndRotation(spawns.FatSpawn.position, spawns.FatSpawn.rotation);
            fatMouseMovement.Initialize(fatMouseVisuals);

            Physics.IgnoreCollision(thinMouseMovement.Collider, fatMouseMovement.Collider);

            var playerGo = new GameObject("Two mouse player");
            var player = playerGo.AddComponent<TwoMousePlayer>();
            GameContainer.InjectToInstance(player);
            player.Initialize(fatMouseMovement, thinMouseMovement);

            GameContainer.InGame.Register<IPlayer>(player);
        }

        public override void Dispose()
        {
        }
    }
}