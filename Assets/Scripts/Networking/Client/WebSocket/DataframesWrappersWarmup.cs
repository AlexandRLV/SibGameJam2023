using System.Linq;
using System.Reflection;
using NetFrame;
using Networking.Dataframes;
using Networking.Dataframes.InGame;
using Networking.Dataframes.InGame.LevelObjects;

namespace Networking
{
    public static class DataframesWrappersWarmup
    {
        public static void WarmupDataframesWrappers()
        {
            DataframeWrapper<DisconnectByReasonDataframe>.Warmup();
            DataframeWrapper<PlayerInfoDataframe>.Warmup();
            DataframeWrapper<PlayerInfoReceivedDataframe>.Warmup();
            DataframeWrapper<PlayerInfoRequestDataframe>.Warmup();
            DataframeWrapper<CreateRoomDataframe>.Warmup();
            DataframeWrapper<JoinedRoomDataframe>.Warmup();
            DataframeWrapper<JoinRoomFailedDataframe>.Warmup();
            DataframeWrapper<JoinRoomRequestDataframe>.Warmup();
            DataframeWrapper<LeaveRoomDataframe>.Warmup();
            DataframeWrapper<PlayerJoinedRoomDataframe>.Warmup();
            DataframeWrapper<PlayerLeftRoomDataframe>.Warmup();
            DataframeWrapper<PlayerReadyStateDataframe>.Warmup();
            DataframeWrapper<RoomInfoDataframe>.Warmup();
            DataframeWrapper<RoomPrepareToPlayDataframe>.Warmup();
            DataframeWrapper<RoomsListDataframe>.Warmup();
            DataframeWrapper<RoomsRequestDataframe>.Warmup();
            DataframeWrapper<ActivateMouseTrapDataframe>.Warmup();
            DataframeWrapper<EnemyAlertPlayerDataframe>.Warmup();
            DataframeWrapper<EnemyDetectPlayerDataframe>.Warmup();
            DataframeWrapper<GameFinishedDataframe>.Warmup();
            DataframeWrapper<InteractedWithObjectDataframe>.Warmup();
            DataframeWrapper<LoseGameDataframe>.Warmup();
            DataframeWrapper<PlayerEffectStateDataframe>.Warmup();
            DataframeWrapper<PlayerPositionDataframe>.Warmup();
            DataframeWrapper<PushablePositionDataframe>.Warmup();
            DataframeWrapper<SetCurrentTickDataframe>.Warmup();
            DataframeWrapper<SkipIntroDataframe>.Warmup();
            DataframeWrapper<Vector3Dataframe>.Warmup();
            DataframeWrapper<CreateNetworkObjectDataframe>.Warmup();
            DataframeWrapper<DestroyNetworkObjectDataframe>.Warmup();
            DataframeWrapper<NetworkObjectInterpolatePositionDataframe>.Warmup();
            DataframeWrapper<NetworkObjectSetTickDataframe>.Warmup();
        }
    }
}