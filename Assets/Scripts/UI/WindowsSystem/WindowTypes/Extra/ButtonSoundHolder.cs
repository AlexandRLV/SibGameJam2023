﻿using Common;
using GameCore.Sounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class ButtonSoundHolder : MonoBehaviour, IPointerEnterHandler
    {
        private static SoundType[] _hoverSounds = new[] { SoundType.Hover1, SoundType.Hover2, SoundType.Hover3 };
        
        private bool _hasButton;
        
        private void Start()
        {
            var button = GetComponent<Button>();
            if (button == null) return;

            _hasButton = true;
            button.onClick.AddListener(() =>
            {
                GameContainer.Common.Resolve<SoundService>().PlaySound(SoundType.Click);
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_hasButton)
                GameContainer.Common.Resolve<SoundService>().PlayRandomSound(_hoverSounds);
        }
    }
}