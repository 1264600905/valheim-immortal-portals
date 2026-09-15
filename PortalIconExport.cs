using System.Collections;
using System.IO;
using BepInEx;
using UnityEngine;

namespace ImmortalPortals
{
    /// <summary>
    /// 打包辅助：读取游戏内原版传送门的建造图标（Piece.m_icon），
    /// 裁剪出精灵区域并导出为 PNG，输出到插件目录：
    ///   ImmortalPortals/portal_wood-icon.png
    ///   ImmortalPortals/portal_stone-icon.png
    /// 用于生成 Thunderstore 发布用的 icon.png；可在配置中关闭。
    /// </summary>
    internal static class PortalIconExport
    {
        internal static void Schedule()
        {
            if (!ImmortalPortalsPlugin.ExportPortalIcons.Value)
            {
                return;
            }

            ImmortalPortalsPlugin.Instance.StartCoroutine(ExportWhenReady());
        }

        private static IEnumerator ExportWhenReady()
        {
            // 等世界加载完成（ZNetScene 与传送门预制体就绪）
            yield return new WaitUntil(() => ZNetScene.instance != null
                                             && ZNetScene.instance.GetPrefab(
                                                 PortalProtection.WoodPortalPrefab) != null);

            Export(PortalProtection.WoodPortalPrefab);
            Export(PortalProtection.StonePortalPrefab);
        }

        private static void Export(string prefabName)
        {
            try
            {
                GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
                if (prefab == null)
                {
                    IplLog.Warn($"图标导出跳过：未找到预制体 {prefabName}");
                    return;
                }

                Piece piece = prefab.GetComponent<Piece>();
                Sprite icon = piece != null ? piece.m_icon : null;
                if (icon == null)
                {
                    IplLog.Warn($"图标导出跳过：{prefabName} 没有 m_icon");
                    return;
                }

                Texture2D cropped = CropSprite(icon);
                if (cropped == null)
                {
                    IplLog.Warn($"图标导出跳过：{prefabName} 纹理裁剪失败");
                    return;
                }

                int width = cropped.width;
                int height = cropped.height;
                byte[] png = cropped.EncodeToPNG();
                Object.Destroy(cropped);

                if (png == null || png.Length == 0)
                {
                    IplLog.Warn($"图标导出失败：{prefabName} PNG 数据为空");
                    return;
                }

                string dir = Path.GetDirectoryName(ImmortalPortalsPlugin.Instance.Info.Location);
                string path = Path.Combine(dir ?? ".", prefabName + "-icon.png");
                File.WriteAllBytes(path, png);
                IplLog.Info($"图标已导出: {path}（{width}x{height}, {png.Length} bytes）");
            }
            catch (System.Exception e)
            {
                IplLog.Warn($"图标导出异常（{prefabName}）: {e.Message}");
            }
        }

        /// <summary>把精灵纹理（可能是图集）裁剪成独立 Texture2D。</summary>
        private static Texture2D CropSprite(Sprite sprite)
        {
            Texture2D source = sprite.texture;
            if (source == null)
            {
                return null;
            }

            Rect rect = sprite.textureRect;
            int x = Mathf.Clamp(Mathf.RoundToInt(rect.x), 0, source.width - 1);
            int y = Mathf.Clamp(Mathf.RoundToInt(rect.y), 0, source.height - 1);
            int width = Mathf.Clamp(Mathf.RoundToInt(rect.width), 1, source.width - x);
            int height = Mathf.Clamp(Mathf.RoundToInt(rect.height), 1, source.height - y);

            Texture2D readable = MakeReadable(source);
            if (readable == null)
            {
                return null;
            }

            var result = new Texture2D(width, height, TextureFormat.RGBA32, false);
            result.SetPixels(readable.GetPixels(x, y, width, height));
            result.Apply();
            if (readable != source)
            {
                Object.Destroy(readable);
            }

            return result;
        }

        /// <summary>纹理不可读时，经 RenderTexture 拷贝一份可读副本。</summary>
        private static Texture2D MakeReadable(Texture2D source)
        {
            if (source.isReadable)
            {
                return source;
            }

            RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0,
                RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            RenderTexture previous = RenderTexture.active;
            try
            {
                Graphics.Blit(source, rt);
                RenderTexture.active = rt;
                var readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
                readable.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
                readable.Apply();
                return readable;
            }
            finally
            {
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rt);
            }
        }
    }
}
