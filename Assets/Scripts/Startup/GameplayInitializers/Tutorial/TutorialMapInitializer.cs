using Common.DI;
using GameCore.LevelObjects;
using UnityEngine.SceneManagement;

namespace Startup.GameplayInitializers.Tutorial
{
    public class TutorialMapInitializer : InitializerBase
    {
        private const string SceneName = "Level_0_tutorial";
        
        public override void Initialize()
        {
            var service = GameContainer.Create<LevelObjectService>();
            GameContainer.InGame.Register(service);
            
            SceneManager.LoadScene(SceneName);
        }

        public override void Dispose()
        {
            var service = GameContainer.InGame.Resolve<LevelObjectService>();
            service.Dispose();
        }
    }
}