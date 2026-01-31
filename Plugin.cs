using BepInEx;
using HarmonyLib;

namespace poopooVRBetterLeaderboard
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony _harmony;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loading...");

            try
            {
                _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
                
                _harmony.PatchAll();

                SpriteLoader.Instance.LoadSprites();

                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} loaded successfully!");
                Logger.LogInfo($"Leaderboard enhancement active - FPS and ping will be displayed");
                Logger.LogInfo($"Loading sprites from server...");
                Logger.LogInfo($"Version: {PluginInfo.PLUGIN_VERSION}");
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Failed to load plugin {PluginInfo.PLUGIN_GUID}: {ex}");
                Logger.LogError($"Stack trace: {ex.StackTrace}");
                
                enabled = false;
            }
        }

        private void Update()
        {
            try
            {
                foreach (GorillaScoreBoard scoreboard in GorillaScoreboardTotalUpdater.allScoreboards)
                {
                    if (scoreboard.boardText != null)
                    {
                        scoreboard.boardText.richText = true;
                        
                        if (SpriteLoader.Instance.IsLoaded && scoreboard.boardText.spriteAsset == null)
                        {
                            scoreboard.boardText.spriteAsset = SpriteLoader.Instance.SpriteAsset;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void OnDestroy()
        {
            if (_harmony != null)
            {
                Logger.LogInfo($"Unpatching {PluginInfo.PLUGIN_GUID}...");
                _harmony.UnpatchSelf();
            }
        }
    }
}
