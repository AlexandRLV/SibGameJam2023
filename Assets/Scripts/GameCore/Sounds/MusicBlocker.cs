using Common.DI;
using UnityEngine;

namespace GameCore.Sounds
{
    public class MusicBlocker : MonoBehaviour
    {
        [Inject] private SoundService _soundService;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
            _soundService.StopMusic();
        }
    }
}