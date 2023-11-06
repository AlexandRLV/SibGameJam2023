﻿using System.Collections;
using UnityEngine.SceneManagement;

namespace Startup.GameplayInitializers
{
    public class GameMapInitializer : IInitializer
    {
        private const string SceneName = "Level_01";
        
        public IEnumerator Initialize()
        {
            var asyncOperation = SceneManager.LoadSceneAsync(SceneName);
            yield return asyncOperation;
        }

        public void Dispose()
        {
            if (SceneManager.GetSceneByName(SceneName).isLoaded)
                SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}