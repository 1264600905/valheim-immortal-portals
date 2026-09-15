using UnityEngine;

namespace ImmortalPortals
{
    /// <summary>
    /// 判定某个 WearNTear 是否属于"受保护的传送门"。
    /// 两个原版预制体：portal_wood（木门）、portal_stone（石门）；
    /// 其他带 TeleportWorld 组件的传送门变体由 ProtectOtherPortals 控制。
    /// </summary>
    internal static class PortalProtection
    {
        internal const string WoodPortalPrefab = "portal_wood";
        internal const string StonePortalPrefab = "portal_stone";

        internal static bool IsProtected(WearNTear wearNTear)
        {
            if (wearNTear == null || !ImmortalPortalsPlugin.Enabled.Value)
            {
                return false;
            }

            if (wearNTear.GetComponent<TeleportWorld>() == null)
            {
                return false;
            }

            string prefab = Utils.GetPrefabName(wearNTear.gameObject.name);
            if (prefab == WoodPortalPrefab)
            {
                return ImmortalPortalsPlugin.ProtectWoodPortal.Value;
            }

            if (prefab == StonePortalPrefab)
            {
                return ImmortalPortalsPlugin.ProtectStonePortal.Value;
            }

            return ImmortalPortalsPlugin.ProtectOtherPortals.Value;
        }
    }
}
