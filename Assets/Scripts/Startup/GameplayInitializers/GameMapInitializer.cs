using System.Collections;
using UnityEngine.SceneManagement;

namespace Startup.GameplayInitializers
{
    public class GameMapInitializer : IInitializer
    {
        private const string SceneName = "CharacterScene";
        
        public IEnumerator Initialize()
        {
            var asyncOperation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            yield return asyncOperation;
        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}