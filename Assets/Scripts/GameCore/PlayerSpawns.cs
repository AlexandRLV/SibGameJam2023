using System.Collections;
using Common;
using Common.DI;
using UnityEngine;

namespace GameCore
{
    [DefaultExecutionOrder(-10)]
    public class PlayerSpawns : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoint;

        public Transform[] SpawnPoints => _spawnPoint;
        
        public void Awake()
        {
            if (GameContainer.InGame != null)
            {
                if (!GameContainer.InGame.CanResolve<PlayerSpawns>())
                    GameContainer.InGame.Register(this);
            }
            else
            {
                StartCoroutine(Register());
            }
        }

        private IEnumerator Register()
        {
            yield return new WaitUntil(() => GameContainer.InGame != null);
            if (!GameContainer.InGame.CanResolve<PlayerSpawns>())
                GameContainer.InGame.Register(this);
        }
    }
}