using System.Collections.Generic;
using UnityEngine;

// Fake save data for the menu demo. Lives for the whole session.
public static class GameData
{
    public class Car
    {
        public string name, maker, carClass, color;
        public int price;
        public float speed, accel, handling, braking; // 0..1
        public bool owned;
    }

    public class Mode
    {
        public string name, icon, description, details;
        public int reward, unlockLevel;
    }

    public class Track
    {
        public string name, area, conditions, bestLap;
        public float lengthKm;
        public int turns, difficulty, unlockLevel; // difficulty 1..4
    }

    public class ShopItem
    {
        public string name, description, sprite;
        public int price, carIndex = -1;
        public bool owned;
    }

    public static string PlayerName = "NIGHTRIDER";
    public static int Level = 9;
    public static float LevelProgress = 0.55f;
    public static int Credits = 62450;

    public static readonly Car[] Cars =
    {
        new Car { name = "KESTREL HR", maker = "Kestrel Motors", carClass = "C", color = "E0412F", price = 0, speed = .52f, accel = .64f, handling = .78f, braking = .60f, owned = true },
        new Car { name = "VANTA S7", maker = "Vanta Automotive", carClass = "B", color = "2E78F0", price = 0, speed = .70f, accel = .68f, handling = .72f, braking = .70f, owned = true },
        new Car { name = "BRUTUS V8", maker = "Brutus Performance", carClass = "A", color = "F2B230", price = 85000, speed = .84f, accel = .88f, handling = .50f, braking = .58f },
        new Car { name = "SPECTRE X", maker = "Spectre Hypercars", carClass = "S", color = "DCE2EC", price = 240000, speed = .97f, accel = .93f, handling = .86f, braking = .90f },
    };

    public static readonly Mode[] Modes =
    {
        new Mode { name = "CIRCUIT", icon = "loop", description = "Three laps around a closed city circuit against seven rivals.", details = "3 LAPS  ·  8 RACERS", reward = 12000 },
        new Mode { name = "SPRINT", icon = "bolt", description = "A flat-out point-to-point dash. No second laps, no second chances.", details = "1 RUN  ·  8 RACERS", reward = 9000 },
        new Mode { name = "DRIFT TRIAL", icon = "drift", description = "Hold the angle and chain corners to build your score multiplier.", details = "2 LAPS  ·  SCORE", reward = 10000 },
        new Mode { name = "TIME ATTACK", icon = "stopwatch", description = "Just you, the clock and the leaderboard ghost.", details = "UNLIMITED  ·  SOLO", reward = 15000, unlockLevel = 12 },
    };

    public static readonly Track[] Tracks =
    {
        new Track { name = "HARBOR LOOP", area = "Port District", conditions = "NIGHT  ·  DRY", bestLap = "1:42.380", lengthKm = 4.2f, turns = 12, difficulty = 1 },
        new Track { name = "NEON BOULEVARD", area = "Downtown", conditions = "NIGHT  ·  WET", bestLap = "2:05.914", lengthKm = 5.8f, turns = 16, difficulty = 2 },
        new Track { name = "CANYON SPINE", area = "Ridge Pass", conditions = "DUSK  ·  DRY", bestLap = "--:--.---", lengthKm = 7.1f, turns = 21, difficulty = 3 },
        new Track { name = "SKYLINE RING", area = "Elevated Highway", conditions = "NIGHT  ·  FOG", bestLap = "--:--.---", lengthKm = 6.4f, turns = 14, difficulty = 4, unlockLevel = 15 },
    };

    public static readonly ShopItem[] Shop =
    {
        new ShopItem { name = "BRUTUS V8", description = "Class A muscle. Huge torque, light grip.", sprite = "car", price = 85000, carIndex = 2 },
        new ShopItem { name = "SPECTRE X", description = "Class S hypercar. Fastest in the city.", sprite = "car", price = 240000, carIndex = 3 },
        new ShopItem { name = "TURBO KIT", description = "+8% acceleration on your active car.", sprite = "bolt", price = 18000 },
        new ShopItem { name = "SPORT TYRES", description = "+6% grip in corners, wet or dry.", sprite = "drift", price = 12000 },
        new ShopItem { name = "CARBON BRAKES", description = "Shorter stopping into hairpins.", sprite = "wrench", price = 9500, owned = true },
        new ShopItem { name = "NITROUS SYSTEM", description = "One extra boost charge per lap.", sprite = "flag", price = 26000 },
    };

    public static int ModeIndex, TrackIndex, CarIndex = 1;

    // last race
    public static int LastPosition = 2;
    public static float LastTime = 300f;

    // settings
    public static readonly Dictionary<string, float> Settings = new Dictionary<string, float>();

    public static Color CarColor(int i) => Palette.Hex(Cars[i].color);

    public static string Money(int v) => v.ToString("N0");
}
