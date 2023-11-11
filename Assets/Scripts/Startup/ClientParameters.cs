using UnityEngine;

namespace Startup
{
    [CreateAssetMenu(fileName = "Client Parameters")]
    public class ClientParameters : ScriptableObject
    {
        public string Ip => isLocal ? "127.0.0.1" : ipAddress;
        
        [SerializeField] public string ipAddress;
        [SerializeField] public bool isLocal;
        [SerializeField] public int port;
    }
}