using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BepInEx;
using System.IO;
using BepInEx.Configuration;
using LethalLib.Modules;

namespace SkullEnemy
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class SkullEnemyPlugin : BaseUnityPlugin
    {
        public static SkullEnemyPlugin Instance;

        public ConfigEntry<float> ConfigMovementSpeed;
        public ConfigEntry<float> ConfigRotationSpeed;
        public ConfigEntry<bool> ConfigCanOnlyKillTargetPlayer;

        private static readonly Dictionary<Levels.LevelTypes, int> DefaultLevelRarities = new()
        {
            [Levels.LevelTypes.RendLevel] = 66,
            [Levels.LevelTypes.TitanLevel] = 66,
            [Levels.LevelTypes.DineLevel] = 66
        };

        private void Awake()
        {
            Instance = this;

            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyLocation.IsNullOrWhiteSpace())
            {
                return;
            }

            var assetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation!, "skullassets"));
            if (assetBundle == null)
            {
                Logger.LogError("Failed to load custom assets");
                return;
            }

            ConfigMovementSpeed = Config.Bind("General", "MovementSpeed", 2f,
                "The speed at which the enemy moves towards its target.");
            ConfigRotationSpeed = Config.Bind("General", "RotationSpeed", 2f,
                "The speed at which the enemy rotates towards its target.");
            ConfigCanOnlyKillTargetPlayer = Config.Bind("General", "CanOnlyKillTargetPlayer", true,
                "If enabled, the enemy will only be able to kill its target player.");

            var levelRarities = new Dictionary<Levels.LevelTypes, int>();
            foreach (Levels.LevelTypes levelType in Enum.GetValues(typeof(Levels.LevelTypes)))
            {
                if (!Levels.LevelTypes.Vanilla.HasFlag(levelType) || levelType == Levels.LevelTypes.Vanilla)
                    continue;

                var configEntry = Config.Bind("Spawning", $"{levelType}Rarity",
                    DefaultLevelRarities.GetValueOrDefault(levelType, 0), $"The spawn rarity on {levelType}.");
                levelRarities.Add(levelType, configEntry.Value);
            }

            var skullEnemyType = assetBundle.LoadAsset<EnemyType>("SkullEnemy");
            var skullTerminalNode = assetBundle.LoadAsset<TerminalNode>("SkullTerminalNode");
            var skullTerminalKeyword = assetBundle.LoadAsset<TerminalKeyword>("SkullTerminalKeyword");

            NetworkPrefabs.RegisterNetworkPrefab(skullEnemyType.enemyPrefab);
            Enemies.RegisterEnemy(skullEnemyType, levelRarities, null, skullTerminalNode, skullTerminalKeyword);

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}