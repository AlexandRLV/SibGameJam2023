using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Startup.Editor
{
    [InitializeOnLoad]
    public class SceneBootstrapper
    {
        private const string PreviousSceneKey = "PreviousScene";
        private const string ShouldLoadBootstrapKey = "LoadBootstrapScene";

        private const string LoadBootstrapMenuLabel = "Tools/Load Bootstrap Scene On Play";
        private const string DontLoadBootstrapMenuLabel = "Tools/Don't Load Bootstrap Scene On Play";
        private const string OpenBootstrapSceneLabel = "Tools/Open Bootstrap Scene";

        private static string BootstrapScene => EditorBuildSettings.scenes[0].path;

        private static string PreviousScene
        {
            get => EditorPrefs.GetString(PreviousSceneKey);
            set => EditorPrefs.SetString(PreviousSceneKey, value);
        }

        private static bool ShouldLoadBootstrapScene
        {
            get => EditorPrefs.GetBool(ShouldLoadBootstrapKey, true);
            set => EditorPrefs.SetBool(ShouldLoadBootstrapKey, value);
        }

        static SceneBootstrapper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (!ShouldLoadBootstrapScene) return;

            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                PreviousScene = SceneManager.GetActiveScene().path;
                
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() && IsSceneInBuildSettings(BootstrapScene))
                    EditorSceneManager.OpenScene(BootstrapScene);
            }
            else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                if (!string.IsNullOrEmpty(PreviousScene))
                    EditorSceneManager.OpenScene(PreviousScene);
            }
        }

        [MenuItem(LoadBootstrapMenuLabel)]
        private static void EnableBootstrapper()
        {
            ShouldLoadBootstrapScene = true;
        }

        [MenuItem(LoadBootstrapMenuLabel, true)]
        private static bool ValidateEnableBootstrapper()
        {
            return !ShouldLoadBootstrapScene;
        }

        // disables the behavior if selected.
        [MenuItem(DontLoadBootstrapMenuLabel)]
        private static void DisableBootstrapper()
        {
            ShouldLoadBootstrapScene = false;
        }

        [MenuItem(DontLoadBootstrapMenuLabel, true)]
        private static bool ValidateDisableBootstrapper()
        {
            return ShouldLoadBootstrapScene;
        }

        [MenuItem(OpenBootstrapSceneLabel)]
        private static void OpenBootstrapScene()
        {
            EditorSceneManager.OpenScene(BootstrapScene);
        }

        private static bool IsSceneInBuildSettings(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                return false;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == scenePath)
                    return true;
            }

            return false;
        }
    }
}