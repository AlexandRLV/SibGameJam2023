using Common.DI;
using Networking;
using UnityEngine;

namespace GameCore.NetworkObjects
{
    [DisallowMultipleComponent]
    public class NetworkObject : MonoBehaviour
    {
        public int Id { get; set; }
        public bool IsOnline { get; set; }
        public bool Teleported { get; set; }
        public NetworkObjectType Type => _type;

        [SerializeField] private NetworkObjectType _type;

        [Inject] private NetworkObjectsController _controller;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
            _controller.RegisterObject(this);
        }
    }
}