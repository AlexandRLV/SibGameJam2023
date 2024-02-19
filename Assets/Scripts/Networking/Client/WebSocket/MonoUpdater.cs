using System;
using UnityEngine;

namespace Networking.Client.WebSocket
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