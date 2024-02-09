using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BepInEx;
using LethalLib.Modules;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using Serilog;

namespace Skull
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class SkullPlugin : BaseUnityPlugin
    {
        public static readonly ConfigFile ConfigFile =
            new(Path.Combine(Paths.ConfigPath, $"{PluginInfo.PLUGIN_NAME}.cfg"), true);

        private void Awake()
        {
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

            ConfigFile.Bind("Movement", "MovementSpeed", 2f, "Movement speed of the enemy");
            ConfigFile.Bind("Movement", "RotationSpeed", 2f, "Rotation speed of the enemy");

            var skullEnemy = assetBundle.LoadAsset<EnemyType>("SkullEnemy");
            var terminalNode = assetBundle.LoadAsset<TerminalNode>("SkullTerminalNode");
            var terminalKeyword = assetBundle.LoadAsset<TerminalKeyword>("SkullTerminalKeyword");

            NetworkPrefabs.RegisterNetworkPrefab(skullEnemy.enemyPrefab);

            List<ConfigEntry<int>> entries =
            [
                ConfigFile.Bind("Rarity", Levels.LevelTypes.ExperimentationLevel.ToString(), 0),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.AssuranceLevel.ToString(), 0),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.VowLevel.ToString(), 0),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.OffenseLevel.ToString(), 0),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.MarchLevel.ToString(), 0),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.RendLevel.ToString(), 60),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.DineLevel.ToString(), 65),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.TitanLevel.ToString(), 70),
                ConfigFile.Bind("Rarity", Levels.LevelTypes.Modded.ToString(), 0)
            ];
            
            foreach (var configEntry in entries)
            {
                Logger.LogInfo(configEntry.Definition.Key + " " + configEntry.Value);
                Enemies.RegisterEnemy(skullEnemy, configEntry.Value,  Enum.Parse<Levels.LevelTypes>(configEntry.Definition.Key), terminalNode, terminalKeyword);
            }
            
            // Required by Unity Netcode Patcher
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

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
    
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void PostAwake()
        {
            foreach (var selectableLevel in StartOfRound.Instance.levels)
            {
                foreach (var spawnableEnemyWithRarity in selectableLevel.Enemies)
                {
                    Debug.Log(
                        $"{selectableLevel.PlanetName}: {spawnableEnemyWithRarity.enemyType.enemyName} [{spawnableEnemyWithRarity.rarity}]");
                }
            }
        }
    }
}