using Common.DI;
using GameCore.LevelObjects.Abstract;
using GameCore.Sounds;
using Networking;
using Networking.Client;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.LevelObjects.TriggerObjects
{
    public class Mousetrap : BaseTriggerObject, ICheckPositionObject
    {
        public Vector3 CheckPosition => transform.position;
        
        [SerializeField] private GameObject cheese;

        [Inject] private LevelObjectService _levelObjectService;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;

        private void Start()
        {
            GameContainer.InjectToInstance(this);
            _levelObjectService.RegisterMousetrap(this);
        }

        private void OnDestroy()
        {
            _levelObjectService.UnregisterMousetrap(this);
        }

        public void Activate()
        {
            if (IsUsed) return;
            
            soundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Destroy(cheese);
            
            IsUsed = true;
        }

        protected override void OnPlayerEnter()
        {
            if (IsUsed) return;
            
            soundService.PlayRandomSound(SoundType.Mousetrap1, SoundType.Mousetrap2, SoundType.Mousetrap3);
            Destroy(cheese);
            
            Movement.MoveValues.IsHit = true;
            Movement.Damage();
            
            IsUsed = true;
            
            if (!_gameClientData.IsConnected) return;

            var dataframe = new ActivateMouseTrapDataframe
            {
                mousetrapPosition = CheckPosition,
            };
            _gameClient.Send(ref dataframe);
        }
    }
}