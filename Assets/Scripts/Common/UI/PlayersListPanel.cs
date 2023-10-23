using System.Text;
using SibGameJam.Common.LocalMessages;
using SibGameJam.Common.LocalMessages.MessageTypes;
using TMPro;
using UnityEngine;

namespace SibGameJam.Common.UI
{
    public class PlayersListPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playersListText;

        private StringBuilder _stringBuilder = new();
        
        private void Start()
        {
            LocalMessageBroker.Subscribe<UpdatePlayersListMessage>(OnUpdatePlayersListRequest);
            UpdatePlayersList();
        }

        private void OnDestroy()
        {
            LocalMessageBroker.Unsubscribe<UpdatePlayersListMessage>(OnUpdatePlayersListRequest);
        }

        private void OnUpdatePlayersListRequest(ref UpdatePlayersListMessage message)
        {
            UpdatePlayersList();
        }

        private void UpdatePlayersList()
        {
            _stringBuilder.Clear();
            foreach (var player in GameData.Players)
            {
                _stringBuilder.Append(player.Key);
                _stringBuilder.Append(": ");
                _stringBuilder.Append(player.Value.Name);
                _stringBuilder.Append("\n");
            }

            _playersListText.text = _stringBuilder.ToString();
        }
    }
}