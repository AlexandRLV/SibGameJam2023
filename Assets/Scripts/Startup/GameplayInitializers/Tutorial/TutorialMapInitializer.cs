using System.Collections;
using Common.DI;
using GameCore.LevelObjects;
using UnityEngine.SceneManagement;

namespace Startup.GameplayInitializers.Tutorial
{
    public class TutorialMapInitializer : IInitializer
    {
        private const string SceneName = "Level_0_tutorial";
        
        public IEnumerator Initialize()
        {
            var service = GameContainer.Create<LevelObjectService>();
            GameContainer.InGame.Register(service);
            
            var asyncOperation = SceneManager.LoadSceneAsync(SceneName);
            yield return asyncOperation;
        }

        public void Dispose()
        {
            var service = GameContainer.InGame.Resolve<LevelObjectService>();
            service.Dispose();
            
            if (SceneManager.GetSceneByName(SceneName).isLoaded)
                SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}