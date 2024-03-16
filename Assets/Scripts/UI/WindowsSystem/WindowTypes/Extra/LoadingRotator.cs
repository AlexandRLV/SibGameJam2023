using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    public class LoadingRotator : MonoBehaviour
    {
        [SerializeField] private Image _loadingImage;
        [SerializeField] private float _oneSpriteInterval;
        [SerializeField] private float _rotationAngle;
        [SerializeField] private Sprite[] _animationSprites;

        private int _spriteId;
        private float _timer;

        private void Start()
        {
            _loadingImage.sprite = _animationSprites[0];
            _timer = _oneSpriteInterval;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0) return;

            _timer += _oneSpriteInterval;
            _spriteId++;
            if (_spriteId >= _animationSprites.Length)
            {
                _spriteId = 0;
                transform.Rotate(0f, 0f, -_rotationAngle);
            }

            _loadingImage.sprite = _animationSprites[_spriteId];
        }
    }
}