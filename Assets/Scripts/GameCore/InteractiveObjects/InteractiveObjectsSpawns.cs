using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class InteractiveObjectsSpawns : MonoBehaviour
    {
        public List<Transform> spawnPoints;

        private void Start()
        {
            foreach (Transform point in transform)
            {
                spawnPoints.Add(point);
            }
            if (GameContainer.InGame != null)
                GameContainer.InGame.Register(this);
            else
                StartCoroutine(Register());
        }

        private IEnumerator Register()
        {
            yield return new WaitUntil(() => GameContainer.InGame != null);
            GameContainer.InGame.Register(this);
        }
    }
}