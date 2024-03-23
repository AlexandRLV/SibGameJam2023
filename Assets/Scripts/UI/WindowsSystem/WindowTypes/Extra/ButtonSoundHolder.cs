using Common.DI;
using GameCore.Sounds;
using GameCore.Sounds.Playback;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class ButtonSoundHolder : MonoBehaviour, IPointerEnterHandler
    {
        [Inject] private SoundService _soundService;
        
        private bool _hasButton;
        
        private void Start()
        {
            GameContainer.InjectToInstance(this);
            
            var button = GetComponent<Button>();
            if (button == null) return;

            _hasButton = true;
            button.onClick.AddListener(() =>
            {
                _soundService.PlaySound(SoundType.Click);
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_hasButton)
                _soundService.PlaySound(SoundType.Hover);
        }
    }
}