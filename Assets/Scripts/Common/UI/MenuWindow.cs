using NetFrame.Utils;
using SibGameJam.Client;
using SibGameJam.Server;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SibGameJam.Common.UI
{
    public class MenuWindow : MonoBehaviour
    {
        [SerializeField] private Button _startServerButton;
        [SerializeField] private Button _startClientButton;
        [SerializeField] private TMP_InputField _inputField;

        [SerializeField] private ServerController _serverPrefab;
        [SerializeField] private ClientController _clientPrefab;

        private void Awake()
        {
            _startServerButton.onClick.AddListener(() =>
            {
                var go = Instantiate(_serverPrefab);
                DontDestroyOnLoad(go);
                SceneManager.LoadScene("Scenes/NetFrameTest/GameScene");
            });
            
            _startClientButton.onClick.AddListener(() =>
            {
                ClientData.ClientName = _inputField.text;
                var go = Instantiate(_clientPrefab);
                DontDestroyOnLoad(go);
                SceneManager.LoadScene("Scenes/NetFrameTest/GameScene");
            });
        }
    }
}