using System.Collections.Generic;

namespace SKILL_ISSUE.Level
{
    public class WorldProgressionManager
    {
        private readonly HashSet<string> _unlockedLevels;
        private string _currentLevelId;

        public WorldProgressionManager()
        {
            _unlockedLevels = new HashSet<string>();
            _currentLevelId = "";

            // W1_L1 is always unlocked at start
            _unlockedLevels.Add("W1_L1");
        }

        public bool IsLevelUnlocked(string levelId)
        {
            return _unlockedLevels.Contains(levelId);
        }

        public void UnlockLevel(string levelId)
        {
            _unlockedLevels.Add(levelId);
        }

        public void CompleteLevel(string levelId)
        {
            if (!_unlockedLevels.Contains(levelId))
                return;

            // Unlock next level in sequence
            string nextLevel = GetNextLevelId(levelId);
            if (!string.IsNullOrEmpty(nextLevel))
            {
                UnlockLevel(nextLevel);
            }
        }

        public string GetNextLevelId(string currentId)
        {
            // Parse level ID format: W#_L#
            if (string.IsNullOrEmpty(currentId) || currentId.Length < 6)
                return null;

            string[] parts = currentId.Split('_');
            if (parts.Length != 2)
                return null;

            int world = int.Parse(parts[0].Substring(1));
            int level = int.Parse(parts[1].Substring(1));

            // Next level in same world
            if (level < 3)
            {
                return $"W{world}_L{level + 1}";
            }
            // First level of next world
            else if (world < 5)
            {
                return $"W{world + 1}_L1";
            }

            // No more levels
            return null;
        }

        public string CurrentLevelId
        {
            get => _currentLevelId;
            set => _currentLevelId = value;
        }

        public int GetUnlockedCount()
        {
            return _unlockedLevels.Count;
        }
    }
}
