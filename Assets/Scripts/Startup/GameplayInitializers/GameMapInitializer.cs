using Common;
using GameCore.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Startup.GameplayInitializers
{
    public class GameMapInitializer : IInitializer
    {
        public const string SceneName = "";
        
        public void Initialize()
        {
            SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);

            var inputState = new InputState();
            GameContainer.InGame.Register(inputState);
            
            var inputSourcePrefab = Resources.Load<DesktopInputSource>("Input/DesktopInputSource");
            Object.Instantiate(inputSourcePrefab);
        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}