using HarmonyLib;
using Photon.Pun;
using System.Reflection;

namespace poopooVRBetterLeaderboard
{
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine), "UpdatePlayerText")]
    public class LeaderboardPatch
    {
        private static FieldInfo cosmeticStringField = null;
        private static FieldInfo fpsField = null;
        private static FieldInfo velocityHistoryField = null;
        private static PropertyInfo timeProperty = null;
        private static System.Reflection.FieldInfo timeField = null;

        private static string GetPlatform(VRRig rig)
        {
            try
            {
                if (cosmeticStringField == null)
                {
                    cosmeticStringField = typeof(VRRig).GetField("rawCosmeticString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                }

                if (cosmeticStringField != null)
                {
                    string rawCosmeticString = (string)cosmeticStringField.GetValue(rig);
                    
                    if (!string.IsNullOrEmpty(rawCosmeticString))
                    {
                        if (rawCosmeticString.Contains("S. FIRST LOGIN"))
                            return "Steam";
                        else if (rawCosmeticString.Contains("FIRST LOGIN"))
                            return "PC";
                    }
                }
                
                try
                {
                    if (rig.Creator != null)
                    {
                        var player = rig.Creator.GetPlayerRef();
                        if (player != null && player.CustomProperties != null && player.CustomProperties.Count >= 2)
                            return "PC";
                    }
                }
                catch { }
            }
            catch { }

            return "Standalone";
        }

        private static int GetFPS(VRRig rig)
        {
            try
            {
                if (fpsField == null)
                {
                    fpsField = typeof(VRRig).GetField("fps", BindingFlags.Public | BindingFlags.Instance);
                    if (fpsField == null)
                    {
                        fpsField = typeof(VRRig).GetField("fps", BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                }

                if (fpsField != null)
                {
                    object fpsValue = fpsField.GetValue(rig);
                    if (fpsValue != null)
                    {
                        if (fpsValue is int intFps)
                            return intFps;
                        else if (fpsValue is short shortFps)
                            return (int)shortFps;
                    }
                }
            }
            catch { }
            
            return 0;
        }

        private static int GetPing(VRRig rig)
        {
            try
            {
                if (velocityHistoryField == null)
                {
                    velocityHistoryField = typeof(VRRig).GetField("velocityHistoryList", BindingFlags.Public | BindingFlags.Instance);
                    if (velocityHistoryField == null)
                    {
                        velocityHistoryField = typeof(VRRig).GetField("velocityHistoryList", BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                }

                if (velocityHistoryField != null)
                {
                    var velocityHistoryList = velocityHistoryField.GetValue(rig) as System.Collections.IList;
                    
                    if (velocityHistoryList != null && velocityHistoryList.Count > 0)
                    {
                        var firstItem = velocityHistoryList[0];
                        
                        if (firstItem != null)
                        {
                            var itemType = firstItem.GetType();
                            
                            if (timeField == null)
                            {
                                timeField = itemType.GetField("time", BindingFlags.Public | BindingFlags.Instance);
                                if (timeField == null)
                                {
                                    timeField = itemType.GetField("time", BindingFlags.NonPublic | BindingFlags.Instance);
                                }
                            }
                            
                            if (timeField != null)
                            {
                                double time = (double)timeField.GetValue(firstItem);
                                double pingMs = System.Math.Abs((time - PhotonNetwork.Time) * 1000);
                                int ping = (int)System.Math.Clamp(System.Math.Round(pingMs), 0, 9999);
                                
                                if (ping <= 150)
                                    return 4;
                                else if (ping <= 300)
                                    return 3;
                                else if (ping <= 450)
                                    return 2;
                                else
                                    return 1;
                            }
                            
                            if (timeProperty == null)
                            {
                                timeProperty = itemType.GetProperty("time", BindingFlags.Public | BindingFlags.Instance);
                            }
                            
                            if (timeProperty != null)
                            {
                                double time = (double)timeProperty.GetValue(firstItem);
                                double pingMs = System.Math.Abs((time - PhotonNetwork.Time) * 1000);
                                int ping = (int)System.Math.Clamp(System.Math.Round(pingMs), 0, 9999);
                                
                                if (ping <= 150)
                                    return 4;
                                else if (ping <= 300)
                                    return 3;
                                else if (ping <= 450)
                                    return 2;
                                else
                                    return 1;
                            }
                        }
                    }
                }
            }
            catch { }
            
            int localPing = PhotonNetwork.GetPing();
            if (localPing <= 150)
                return 4;
            else if (localPing <= 300)
                return 3;
            else if (localPing <= 450)
                return 2;
            else
                return 1;
        }

        public static void Postfix(GorillaPlayerScoreboardLine __instance)
        {
            try
            {
                VRRig rig = GorillaGameManager.StaticFindRigForPlayer(__instance.linePlayer);
                
                if (rig == null)
                    return;

                int fps = GetFPS(rig);
                
                string originalName = __instance.linePlayer.NickName;
                string enhancedName;

                if (SpriteLoader.Instance.IsLoaded)
                {
                    int pingLevel = GetPing(rig);
                    
                    enhancedName = $"{originalName}<size=50> <sprite name=\"Ping{pingLevel}\">{fps}</size>";
                }
                else
                {
                    enhancedName = $"{originalName}<size=50> {fps}</size>";
                }
                
                __instance.playerNameVisible = enhancedName;
            }
            catch
            {
            }
        }
    }
}
