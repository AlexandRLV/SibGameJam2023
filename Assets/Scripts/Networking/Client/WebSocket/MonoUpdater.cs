using System;
using UnityEngine;

namespace Networking
{
    public class MonoUpdater : MonoBehaviour
    {
        public event Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}