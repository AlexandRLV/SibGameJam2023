using Common.DI;
using UnityEngine;

namespace GameCore.LevelAchievements.AchievementView
{
    public class HpAchievementView : MonoBehaviour
    {
        [Inject] private LevelStatus _levelStatus;
        
        public void Setup()
        {
            GameContainer.InjectToInstance(this);
            gameObject.SetActive(!_levelStatus.damaged);
        }
    }
}