using System;
using UnityEngine;

namespace Features.Service
{
    public enum GameMode
    {
        Campaign,
        Endless,
        Tutorial
    }

    [CreateAssetMenu(fileName = "ServiceConfig", menuName = "Scriptable Objects/ServiceConfig")]
    public class ServiceConfig : ScriptableObject
    {
        [SerializeField] private GameMode gameMode = GameMode.Campaign;
        [SerializeField] private int levelIndex = 0;
        [SerializeField] private LevelData[] levels = Array.Empty<LevelData>();
        [SerializeField] private LevelData tutorialLevel;
        [SerializeField] private int endlessRampOrders = 6;

        public const string HighestCompletedLevelIndexKey = "HighestCompletedLevelIndex";
        public const string EndlessHighScoreKey = "EndlessHighScore";
        public const string CameraSensitivityKey = "CameraSensitivity";
        public const float DefaultCameraSensitivity = 1f;
        public const float CameraSensitivityScale = 16000f;

        public GameMode GameMode => gameMode;

        public void SetGameMode(GameMode value)
        {
            gameMode = value;
        }

        public void SetLevelIndex(int value)
        {
            levelIndex = value;
        }

        public int LevelIndex => levelIndex;

        public LevelData[] Levels => levels;

        public LevelData CurrentLevel => levels[levelIndex];

        public LevelData EndlessLevel => levels[Mathf.Clamp(PlayerPrefs.GetInt(HighestCompletedLevelIndexKey, 0), 0, levels.Length - 1)];

        public LevelData TutorialLevel => tutorialLevel;

        public LevelData ActiveLevel => GameMode switch
        {
            GameMode.Endless => EndlessLevel,
            GameMode.Tutorial => tutorialLevel,
            _ => CurrentLevel
        };

        public int EndlessRampOrders => endlessRampOrders;
    }
}
