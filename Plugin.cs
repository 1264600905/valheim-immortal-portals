using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace ImmortalPortals
{
    /// <summary>
    /// 不朽传送门：让原版两种传送门（木门 portal_wood / 石门 portal_stone）
    /// 免疫一切伤害。怪物与玩家的攻击、火焰、雪灾、火山灰、支撑坍塌等
    /// 都无法再摧毁传送门；建造锤拆除不受影响（拆除走 WearNTear.Remove，
    /// 不经过伤害流程）。
    /// </summary>
    [BepInPlugin(Guid, PluginName, PluginVersion)]
    public class ImmortalPortalsPlugin : BaseUnityPlugin
    {
        public const string Guid = "trigger.valheim.immortalportals";
        public const string PluginName = "Immortal Portals";
        public const string PluginVersion = "0.1.0";

        internal static ImmortalPortalsPlugin Instance;
        internal static ManualLogSource Log;

        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<bool> ProtectWoodPortal;
        internal static ConfigEntry<bool> ProtectStonePortal;
        internal static ConfigEntry<bool> ProtectOtherPortals;
        internal static ConfigEntry<bool> DebugLog;
        internal static ConfigEntry<bool> ExportPortalIcons;

        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            Enabled = Config.Bind("General", "Enabled", true,
                "启用传送门保护（总开关）。");
            ProtectWoodPortal = Config.Bind("General", "ProtectWoodPortal", true,
                "保护木制传送门（portal_wood）。");
            ProtectStonePortal = Config.Bind("General", "ProtectStonePortal", true,
                "保护石制传送门（portal_stone）。");
            ProtectOtherPortals = Config.Bind("General", "ProtectOtherPortals", true,
                "保护其他带 TeleportWorld 组件的传送门（如模组新增的传送门变体）。");
            DebugLog = Config.Bind("General", "DebugLog", false,
                "输出详细调试日志（排查用，默认关闭）。");
            ExportPortalIcons = Config.Bind("General", "ExportPortalIcons", true,
                "启动并进入世界后，把游戏内原版传送门图标导出为 PNG 到插件目录" +
                "（用于生成模组 icon.png，打包完成后可关闭）。");

            IplLog.Info($"{PluginName} v{PluginVersion} 初始化中...");

            _harmony = new Harmony(Guid);
            Patches.Install(_harmony);
            PortalIconExport.Schedule();

            IplLog.Info($"{PluginName} v{PluginVersion} 初始化完成。" +
                        $"Enabled={Enabled.Value}, 木门={ProtectWoodPortal.Value}, " +
                        $"石门={ProtectStonePortal.Value}, 其他={ProtectOtherPortals.Value}");
        }

        private void OnDestroy()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
            }
        }
    }
}
