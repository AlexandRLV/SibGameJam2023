﻿using System.Collections;
using Common;
using Common.DI;
using GameCore.Character.Movement;
using GameCore.Player;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    public class HpBar : MonoBehaviour
    {
        [SerializeField] private GameObject[] _hpItems;

        private IPlayer _player;
        private CharacterMovement _character;
        
        private void OnEnable()
        {
            StartCoroutine(FindPlayer());
        }

        private IEnumerator FindPlayer()
        {
            while (!GameContainer.InGame.CanResolve<IPlayer>())
            {
                yield return null;
            }
            _player = GameContainer.InGame.Resolve<IPlayer>();
            while (_player.CurrentMovement == null)
            {
                yield return null;
            }
            _character = _player.CurrentMovement;
            _character.Lives.LivesChanged += OnLivesChanged;
            
            UpdateLives();
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

            int hpItemId = Mathf.Min(_hpItems.Length - 1, _character.Lives.Lives - 1);
            _hpItems[hpItemId].SetActive(true);
        }
    }
}