using System.Reflection;
using UnityEngine;
using BepInEx;
using LethalLib.Modules;
using System.IO;

namespace Skull
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class SkullPlugin : BaseUnityPlugin
    {
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

            var skull = assetBundle.LoadAsset<EnemyType>("SkullEnemy");
            var terminalNode = assetBundle.LoadAsset<TerminalNode>("SkullTerminalNode");
            var terminalKeyword = assetBundle.LoadAsset<TerminalKeyword>("SkullTerminalKeyword");

            NetworkPrefabs.RegisterNetworkPrefab(skull.enemyPrefab);
            Enemies.RegisterEnemy(skull, 100,
                Levels.LevelTypes.TitanLevel | Levels.LevelTypes.DineLevel | Levels.LevelTypes.RendLevel,
                Enemies.SpawnType.Default, terminalNode, terminalKeyword);

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

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}