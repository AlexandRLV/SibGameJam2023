// using TMPro;
// using UnityEngine;
//
// namespace LocalMessages
// {
//     public class GameFinishedUI : MonoBehaviour, IListener
//     {
//         [SerializeField] private TextMeshProUGUI _resultText;
//         
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
//             if (message is not EvacuatedMessage)
//                 return;
//             
//             var evacuatedMessage = (EvacuatedMessage)message;
//             _resultText.text = evacuatedMessage.LevelCompleteTime.ToString();
//
//             Debug.Log("GameFinished! Creating mission complete ui ");
//         }
//
//         public void OnEvent(EvacuatedMessage message)
//         {
//             
//         }
//     }
// }