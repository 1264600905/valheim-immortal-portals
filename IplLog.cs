using System;

namespace ImmortalPortals
{
    /// <summary>轻量日志包装：Debug 级别受配置开关控制。</summary>
    internal static class IplLog
    {
        internal static void Info(string message)
        {
            ImmortalPortalsPlugin.Log?.LogInfo(message);
        }

        internal static void Warn(string message)
        {
            ImmortalPortalsPlugin.Log?.LogWarning(message);
        }

        internal static void Error(string message, Exception exception = null)
        {
            if (exception == null)
            {
                ImmortalPortalsPlugin.Log?.LogError(message);
            }
            else
            {
                ImmortalPortalsPlugin.Log?.LogError($"{message} {exception}");
            }
        }

        internal static void Debug(string message)
        {
            if (ImmortalPortalsPlugin.DebugLog != null && ImmortalPortalsPlugin.DebugLog.Value)
            {
                ImmortalPortalsPlugin.Log?.LogInfo("[debug] " + message);
            }
        }
    }
}
