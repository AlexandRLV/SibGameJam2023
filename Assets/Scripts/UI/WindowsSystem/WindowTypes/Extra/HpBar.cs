using System.Collections;
using Common;
using GameCore.Character.Movement;
using GameCore.Player;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    public class HpBar : MonoBehaviour
    {
        [SerializeField] private GameObject[] _hpItems;

        private TwoMousePlayer _player;
        private CharacterMovement _character;
        
        private void OnEnable()
        {
            StartCoroutine(FindPlayer());
        }

        private void OnDisable()
        {
            if (_character != null)
                _character.Lives.LivesChanged -= OnLivesChanged;
        }

        private void OnLivesChanged()
        {
            UpdateLives();
        }

        private void UpdateLives()
        {
            foreach (var hpItem in _hpItems)
            {
                hpItem.SetActive(false);
            }
            
            if (_character.Lives.Lives <= 0)
                return;
            
            _hpItems[_character.Lives.Lives - 1].SetActive(true);
        }

        private IEnumerator FindPlayer()
        {
            while (!GameContainer.InGame.CanResolve<TwoMousePlayer>())
            {
                yield return null;
            }
            _player = GameContainer.InGame.Resolve<TwoMousePlayer>();
            while (_player.CurrentCharacter == null)
            {
                yield return null;
            }
            _character = _player.CurrentCharacter;
            _character.Lives.LivesChanged += OnLivesChanged;
            
            UpdateLives();
        }
    }
}