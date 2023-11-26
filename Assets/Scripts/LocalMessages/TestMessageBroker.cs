// using System;
// using System.Collections.Generic;
// using NUnit.Framework.Internal;
// using UnityEngine;
//
// namespace LocalMessages
// {
//     public class Message
//     {
//     }
//
//     public class EvacuatedMessage : Message
//     {
//         public float LevelCompleteTime;
//     }
//
//     public class MouseSavedMessage : Message
//     {
//         public Transform targetMouse;
//     }
//     
//     public interface IListener
//     {
//         public void OnEvent(Message message);
//     }
//     
//     public interface IListener<T>
//     {
//         public void OnEvent<T>(Message message);
//     }
//
//     public enum MessageType
//     {
//         PlayerEvacuated, // сколько времени у игрока ушло на миссию
//         MouseSaved, // трансформ спасённой мыши
//     }
//     
//     public static class TestMessageBroker
//     {
//         public static Dictionary<MessageType, List<IListener>> listeners;
//         
//         public static Dictionary<string, List<IListener>> genericListeners;
//         public static Dictionary<Type, List<IListener>> typedGenericListeners;
//         
//         public static void Test()
//         {
//             TriggerEvent<EvacuatedMessage>();
//             TriggerEvent<MouseSavedMessage>();
//         }
//         
//         public static void TriggerEvent(MessageType eventName, Message message)
//         {
//             var eventListeners = listeners[eventName];
//             foreach (var listener in eventListeners)
//             {
//                 listener.OnEvent(message);
//             }
//         }
//
//         public static void TriggerEvent<T>() where T : Message
//         {
//             var type = typeof(T);
//             var eventListeners = genericListeners[type.Name];
//         }
//         
//         public static void TriggerEvent2<T>(T message) where T : Message
//         {
//             var type = typeof(T);
//             var eventListeners = typedGenericListeners[type];
//             foreach (var listener in eventListeners)
//             {
//                 listener.OnEvent(message);
//             }
//         }
//         
//         public static void TriggerEvent_EvacutedMessage()
//         public static void TriggerEvent_MouseSavedMessage()
//     }
// }