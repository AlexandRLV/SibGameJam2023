// using UnityEngine;
//
// namespace LocalMessages
// {
//     public class Spawner : MonoBehaviour, IListener
//     {
//         private void OnEnable()
//         {
//             TestMessageBroker.listeners[MessageType.PlayerEvacuated].Add(this);
//         }
//
//         private void OnDisable()
//         {
//             TestMessageBroker.listeners[MessageType.PlayerEvacuated].Remove(this);
//         }
//
//         public void OnEvent(Message message)
//         {
//             Debug.Log("GameFinished! Destroying spawner");
//         }
//     }
// }