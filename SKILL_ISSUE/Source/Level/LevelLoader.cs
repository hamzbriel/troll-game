using Microsoft.Xna.Framework;
using SKILL_ISSUE.Traps;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SKILL_ISSUE.Level
{
    public class LevelLoader
    {
        public static LevelData LoadFromJson(string jsonContent)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                return JsonSerializer.Deserialize<LevelData>(jsonContent, options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse level JSON: {ex.Message}");
            }
        }

        public static List<TrapBase> CreateTrapsFromData(List<TrapData> trapDataList)
        {
            var traps = new List<TrapBase>();

            foreach (var data in trapDataList)
            {
                TrapBase trap = data.Type.ToLower() switch
                {
                    "hiddenspikes" => CreateHiddenSpikes(data.Parameters),
                    "fakeplatform" => CreateFakePlatform(data.Parameters),
                    "disappearingfloor" => CreateDisappearingFloor(data.Parameters),
                    "fallingblock" => CreateFallingBlock(data.Parameters),
                    "movingplatform" => CreateMovingPlatform(data.Parameters),
                    "rotatingsaw" => CreateRotatingSaw(data.Parameters),
                    "baitobject" => CreateBaitObject(data.Parameters),
                    "teleporttrap" => CreateTeleportTrap(data.Parameters),
                    "fakeexit" => CreateFakeExit(data.Parameters),
                    _ => null
                };

                if (trap != null)
                {
                    traps.Add(trap);
                }
            }

            return traps;
        }

        private static HiddenSpikes CreateHiddenSpikes(Dictionary<string, object> p)
        {
            var trigger = GetRect(p, "triggerBounds");
            var spike = GetRect(p, "spikeBase");
            return new HiddenSpikes(trigger, spike);
        }

        private static FakePlatform CreateFakePlatform(Dictionary<string, object> p)
        {
            var rect = GetRect(p, "bounds");
            return new FakePlatform(rect);
        }

        private static DisappearingFloor CreateDisappearingFloor(Dictionary<string, object> p)
        {
            var trigger = GetRect(p, "triggerZone");
            var floor = GetRect(p, "floorBounds");
            return new DisappearingFloor(trigger, floor);
        }

        private static FallingBlock CreateFallingBlock(Dictionary<string, object> p)
        {
            var trigger = GetRect(p, "triggerZone");
            var start = GetVector2(p, "startPosition");
            int width = GetInt(p, "width");
            int height = GetInt(p, "height");
            return new FallingBlock(trigger, start, width, height);
        }

        private static MovingPlatform CreateMovingPlatform(Dictionary<string, object> p)
        {
            var start = GetVector2(p, "startPoint");
            var end = GetVector2(p, "endPoint");
            int width = GetInt(p, "width");
            int height = GetInt(p, "height");
            return new MovingPlatform(start, end, width, height);
        }

        private static RotatingSaw CreateRotatingSaw(Dictionary<string, object> p)
        {
            var start = GetVector2(p, "startPoint");
            var end = GetVector2(p, "endPoint");
            int radius = p.ContainsKey("radius") ? GetInt(p, "radius") : 0;
            return new RotatingSaw(start, end, radius);
        }

        private static BaitObject CreateBaitObject(Dictionary<string, object> p)
        {
            var pos = GetVector2(p, "position");
            int size = p.ContainsKey("size") ? GetInt(p, "size") : 16;
            return new BaitObject(pos, size);
        }

        private static TeleportTrap CreateTeleportTrap(Dictionary<string, object> p)
        {
            var trigger = GetRect(p, "triggerZone");
            var dest = GetVector2(p, "destination");
            return new TeleportTrap(trigger, dest);
        }

        private static FakeExit CreateFakeExit(Dictionary<string, object> p)
        {
            var rect = GetRect(p, "bounds");
            Vector2? dest = p.ContainsKey("teleportDestination")
                ? GetVector2(p, "teleportDestination")
                : null;
            return new FakeExit(rect, dest);
        }

        private static Rectangle GetRect(Dictionary<string, object> p, string key)
        {
            if (p.TryGetValue(key, out var value) && value is JsonElement elem)
            {
                return new Rectangle(
                    elem.GetProperty("x").GetInt32(),
                    elem.GetProperty("y").GetInt32(),
                    elem.GetProperty("width").GetInt32(),
                    elem.GetProperty("height").GetInt32()
                );
            }
            return Rectangle.Empty;
        }

        private static Vector2 GetVector2(Dictionary<string, object> p, string key)
        {
            if (p.TryGetValue(key, out var value) && value is JsonElement elem)
            {
                return new Vector2(
                    elem.GetProperty("x").GetSingle(),
                    elem.GetProperty("y").GetSingle()
                );
            }
            return Vector2.Zero;
        }

        private static int GetInt(Dictionary<string, object> p, string key)
        {
            if (p.TryGetValue(key, out var value))
            {
                if (value is JsonElement elem)
                    return elem.GetInt32();
                if (value is int i)
                    return i;
            }
            return 0;
        }
    }
}
