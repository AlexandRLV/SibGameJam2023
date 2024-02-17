using GameCore.LevelAchievements;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    public class MedalView : MonoBehaviour
    {
        [SerializeField] private GameObject _bronze;
        [SerializeField] private GameObject _silver;
        [SerializeField] private GameObject _gold;

        private void Awake()
        {
            SetMedal(MedalType.None);
        }

        public void SetMedal(MedalType type)
        {
            _bronze.SetActive(type == MedalType.Bronze);
            _silver.SetActive(type == MedalType.Silver);
            _gold.SetActive(type == MedalType.Gold);
        }
    }
}