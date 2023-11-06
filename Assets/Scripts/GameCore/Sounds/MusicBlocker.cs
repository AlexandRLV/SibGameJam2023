using Common;
using UnityEngine;

namespace GameCore.Sounds
{
    public class MusicBlocker : MonoBehaviour
    {
        private SoundService SoundService => GameContainer.Common.Resolve<SoundService>();

        private void Awake()
        {
            SoundService.StopMusic();
        }
    }
}