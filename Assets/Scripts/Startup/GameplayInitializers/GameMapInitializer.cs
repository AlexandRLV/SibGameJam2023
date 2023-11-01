using Common;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine.SceneManagement;

namespace Startup.GameplayInitializers
{
    public class GameMapInitializer : IInitializer
    {
        public const string SceneName = "";
        
        public async UniTask Initialize()
        {
            var loadingScreen = GameContainer.Common.Resolve<LoadingScreen>();
            loadingScreen.Active = true;
            
            await SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            
            loadingScreen.Active = false;
        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}