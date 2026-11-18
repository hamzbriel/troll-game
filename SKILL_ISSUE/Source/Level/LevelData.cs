using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SKILL_ISSUE.Level
{
    public class LevelData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Vector2 SpawnPosition { get; set; }
        public Rectangle ExitBounds { get; set; }
        public List<PlatformData> Platforms { get; set; }
        public List<HazardData> Hazards { get; set; }
        public List<TrapData> Traps { get; set; }

        public LevelData()
        {
            Id = "";
            Name = "";
            SpawnPosition = Vector2.Zero;
            ExitBounds = Rectangle.Empty;
            Platforms = new List<PlatformData>();
            Hazards = new List<HazardData>();
            Traps = new List<TrapData>();
        }
    }

    public class PlatformData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class HazardData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class TrapData
    {
        public string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public TrapData()
        {
            Type = "";
            Parameters = new Dictionary<string, object>();
        }
    }
}
