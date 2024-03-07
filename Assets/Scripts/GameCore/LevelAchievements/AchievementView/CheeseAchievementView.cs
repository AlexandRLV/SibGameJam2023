using Common.DI;
using TMPro;
using UnityEngine;

namespace GameCore.LevelAchievements.AchievementView
{
    public class CheeseAchievementView : MonoBehaviour
    {
        [SerializeField] private GameObject _cheeseFullImage;
        [SerializeField] private TextMeshProUGUI _cheeseCount;

        [Inject] private LevelStatus _levelStatus;
        
        public void Setup()
        {
            GameContainer.InjectToInstance(this);
            _cheeseFullImage.SetActive(_levelStatus.cheeseCount > 0);
            _cheeseCount.text = _levelStatus.cheeseCount.ToString();
        }
    }
}