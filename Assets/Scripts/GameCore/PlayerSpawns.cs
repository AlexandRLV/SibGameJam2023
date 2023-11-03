using Common;
using UnityEngine;

namespace GameCore
{
    [DefaultExecutionOrder(-100)]
    public class PlayerSpawns : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        public Transform SpawnPoint => _spawnPoint;
        
        public void Awake()
        {
            GameContainer.InGame.Register(this);
        }
    }
}