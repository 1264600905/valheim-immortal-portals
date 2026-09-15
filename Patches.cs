using System;
using System.Reflection;
using HarmonyLib;

namespace ImmortalPortals
{
    /// <summary>
    /// 只注入一个点：WearNTear.ApplyDamage。
    /// 原版所有伤害路径最终都汇入这里：
    /// - 攻击（怪物/玩家/投射物/火焰/AoE）：WearNTear.RPC_Damage → ApplyDamage；
    /// - 环境磨损（无支撑/雪/火山灰/岩浆等）：WearNTear.UpdateWear → ApplyDamage。
    /// 拦截后传送门不再扣血，因此不会被任何伤害摧毁；建造锤拆除走
    /// RPC_Remove → Destroy，不经过本方法，所以仍可正常拆除。
    /// </summary>
    internal static class Patches
    {
        internal static void Install(Harmony harmony)
        {
            var target = AccessTools.Method(typeof(WearNTear), nameof(WearNTear.ApplyDamage),
                new[] { typeof(float), typeof(HitData) });

            TryPatch(harmony, target, HarmonyMethodFor(nameof(ApplyDamage_Prefix)),
                "WearNTear.ApplyDamage");
        }

        /// <summary>返回 false 跳过原方法：受保护的传送门不受任何伤害。</summary>
        private static bool ApplyDamage_Prefix(WearNTear __instance, HitData hitData)
        {
            if (!PortalProtection.IsProtected(__instance))
            {
                return true;
            }

            if (hitData != null)
            {
                IplLog.Debug($"已拦截对 {__instance.gameObject.name} 的伤害 " +
                             $"{hitData.GetTotalDamage():0.#}（{DescribeAttacker(hitData)}）");
            }

            return false;
        }

        private static string DescribeAttacker(HitData hitData)
        {
            Character attacker = hitData.GetAttacker();
            return attacker != null ? attacker.m_name : hitData.m_hitType.ToString();
        }

        private static bool TryPatch(Harmony harmony, MethodBase target, HarmonyMethod prefix,
            string label)
        {
            if (target == null)
            {
                IplLog.Error($"未找到目标方法，传送门保护不生效: {label}");
                return false;
            }

            try
            {
                harmony.Patch(target, prefix: prefix);
                IplLog.Info($"已注入: {label}");
                return true;
            }
            catch (Exception e)
            {
                IplLog.Error($"注入失败 {label}: ", e);
                return false;
            }
        }

        private static HarmonyMethod HarmonyMethodFor(string name)
        {
            var method = AccessTools.Method(typeof(Patches), name);
            if (method == null)
            {
                throw new MissingMethodException($"Patches.{name} 不存在");
            }

            return new HarmonyMethod(method);
        }
    }
}
