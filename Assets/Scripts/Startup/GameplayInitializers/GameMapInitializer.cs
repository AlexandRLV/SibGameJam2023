using System.Collections;
using Common;
using GameCore.Input;
using UnityEngine;
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