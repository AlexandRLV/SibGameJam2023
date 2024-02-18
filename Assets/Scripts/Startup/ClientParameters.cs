using UnityEngine;

namespace Startup
{
    [CreateAssetMenu(menuName = "Configs/Client Parameters")]
    public class ClientParameters : ScriptableObject
    {
        public string Ip => isLocal ? "127.0.0.1" : ipAddress;

        [SerializeField] public string ipAddress;
        [SerializeField] public bool isLocal;
        [SerializeField] public int port;
        [SerializeField] public int webSocketPort;
        [SerializeField] public float roomsRequestInterval;
    }
}