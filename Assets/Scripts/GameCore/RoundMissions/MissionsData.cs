using UnityEngine;

namespace GameCore.RoundMissions
{
    [CreateAssetMenu(fileName = "Missions Data")]
    public class MissionsData : ScriptableObject
    {
        [Header("Миссия - спасти агентов")]
        [SerializeField] public string saveAgentsText;
        [SerializeField] public int agentsToSave;
        
        [Header("Миссия - найти кактус")]
        [SerializeField] public string findCactusText;

        [Header("Миссия - выберитесь в зону эвакуации")]
        [SerializeField] public string evacuationText;
    }
}