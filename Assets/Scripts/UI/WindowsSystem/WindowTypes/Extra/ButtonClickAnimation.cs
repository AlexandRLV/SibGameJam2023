using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class ButtonClickAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image _buttonImage;
        
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _hoveredSprite;
        [SerializeField] private Sprite _pressedSprite;
        
        [SerializeField] private GameObject _selectedIndicator;

        private bool _isHovering;
        private bool _isPressing;

        public void SetSelected(bool state)
        {
            _selectedIndicator.SetActive(state);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            _isPressing = false;
            UpdateSprite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            _isPressing = false;
            UpdateSprite();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isHovering = true;
            _isPressing = true;
            UpdateSprite();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHovering = true;
            _isPressing = false;
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            _buttonImage.sprite = _isPressing
                ? _pressedSprite
                : _isHovering
                    ? _hoveredSprite
                    : _defaultSprite;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _buttonImage = GetComponent<Image>();
        }
#endif
    }
}