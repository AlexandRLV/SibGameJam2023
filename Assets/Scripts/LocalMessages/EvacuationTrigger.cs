// using System.Collections.Generic;
// using UnityEngine;
//
// namespace LocalMessages
// {
//     public class EvacuationTrigger : MonoBehaviour
//     {
//         private float _timer;
//
//         private void Update()
//         {
//             _timer += Time.deltaTime;
//         }
//
//         public void PlayerEvacuted()
//         {
//             var message = new EvacuatedMessage
//             {
//                 LevelCompleteTime = _timer
//             };
//             TestMessageBroker.TriggerEvent(MessageType.PlayerEvacuated, message);
//         }
//     }
// }