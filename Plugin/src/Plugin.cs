using System.Reflection;
using UnityEngine;
using BepInEx;
using LethalLib.Modules;
using static LethalLib.Modules.Levels;
using static LethalLib.Modules.Enemies;
using System.IO;

namespace Skull
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class SkullPlugin : BaseUnityPlugin
    {
        private static AssetBundle _mainAssetBundle;

        private void Awake()
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyLocation.IsNullOrWhiteSpace())
            {
                return;
            }

            _mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation!, "skullassets"));
            if (_mainAssetBundle == null)
            {
                Logger.LogError("Failed to load custom assets");
                return;
            }

            var skull = _mainAssetBundle.LoadAsset<EnemyType>("SkullEnemy");
            var terminalNode = _mainAssetBundle.LoadAsset<TerminalNode>("SkullTerminalNode");
            var terminalKeyword = _mainAssetBundle.LoadAsset<TerminalKeyword>("SkullTerminalKeyword");

            NetworkPrefabs.RegisterNetworkPrefab(skull.enemyPrefab);
            RegisterEnemy(skull, 100, LevelTypes.TitanLevel | LevelTypes.DineLevel | LevelTypes.RendLevel,
                SpawnType.Default, terminalNode, terminalKeyword);

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