using System;
using UnityEngine;

namespace Features.Service
{
    public enum GameMode
    {
        Campaign,
        Endless
    }

    [CreateAssetMenu(fileName = "ServiceConfig", menuName = "Scriptable Objects/ServiceConfig")]
    public class ServiceConfig : ScriptableObject
    {
        [SerializeField] private GameMode gameMode = GameMode.Campaign;
        [SerializeField] private int levelIndex = 0;
        [SerializeField] private LevelData[] levels = Array.Empty<LevelData>();

        public GameMode GameMode => gameMode;

        public int LevelIndex => levelIndex;

        public LevelData[] Levels => levels;

        public LevelData CurrentLevel => levels[levelIndex];
    }
}
