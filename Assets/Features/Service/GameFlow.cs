using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.Service
{
    public static class GameFlow
    {
        public const string MainMenuScene = "MainMenuScene";
        public const string ServiceScene = "ServiceScene";

        public static void LoadMainMenu()
        {
            SceneManager.LoadScene(MainMenuScene);
        }

        public static void StartGame(ServiceConfig config, GameMode gameMode, int levelIndex)
        {
            config.SetGameMode(gameMode);
            if (gameMode == GameMode.Campaign)
            {
                config.SetLevelIndex(levelIndex);
            }
            SceneManager.LoadScene(ServiceScene);
        }
    }
}
