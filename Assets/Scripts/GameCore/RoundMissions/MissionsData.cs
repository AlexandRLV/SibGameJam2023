using UnityEngine;

namespace GameCore.RoundMissions
{
    [CreateAssetMenu(menuName = "Configs/Missions Data")]
    public class MissionsData : ScriptableObject
    {
        [Header("Миссия - спасти агентов")]
        [SerializeField] public string saveAgentsLocalizationKey;
        [SerializeField] public string agentsSavedLocalizationKey;
        [SerializeField] public string saveAgentsText;
        [SerializeField] public int agentsToSave;
        
        [Header("Миссия - найти кактус")]
        [SerializeField] public string findCactusLocalizationKey;
        [SerializeField] public string cactusFoundLocalizationKey;
        [SerializeField] public string findCactusText;

        [Header("Миссия - выберитесь в зону эвакуации")]
        [SerializeField] public string evacuationLocalizationKey;
        [SerializeField] public string evacuatedLocalizationKey;
        [SerializeField] public string evacuationText;
    }
}