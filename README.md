# Immortal Portals

A tiny Valheim mod: the two vanilla portals (wood and stone) become immune to **all damage** — monsters, players, fire, snow, ash and support collapse can no longer destroy them. The build hammer still removes them normally.

> Localization: English / Simplified Chinese / Traditional Chinese

---

## Features

- **Both vanilla portals protected**: `portal_wood` (Portal) and `portal_stone` (Stone portal).
- **Immune to every damage path**:
  - monster and player attacks, projectiles, fire, AoE explosions;
  - environmental wear: support collapse, Deep North heavy snow, Ashlands ash and lava, persistent events.
- **Build hammer still works**: removal uses a separate code path, so you can always dismantle your portal and get the materials back.
- **Applies immediately to existing saves**: no health changes, no save data touched.
- **Modded portal variants** (anything using the vanilla `TeleportWorld` component) are protected too — configurable.

## How It Works

All damage to a buildable piece funnels into `WearNTear.ApplyDamage` (attack damage via `RPC_Damage`, environmental wear via `UpdateWear`). The mod Harmony-patches that single method and skips it for portals. Hammer removal goes through `WearNTear.Remove` → `RPC_Remove` → `Destroy`, which never touches `ApplyDamage`, so it keeps working.

## Configuration

File: `BepInEx/config/trigger.valheim.immortalportals.cfg` (ConfigurationManager supported).

| Section | Key | Default | Description |
|---|---|---|---|
| General | Enabled | true | Master switch |
| General | ProtectWoodPortal | true | Protect the wooden portal (`portal_wood`) |
| General | ProtectStonePortal | true | Protect the stone portal (`portal_stone`) |
| General | ProtectOtherPortals | true | Protect other portals using the vanilla `TeleportWorld` component (modded variants) |
| General | DebugLog | false | Log blocked damage (troubleshooting) |

## Installation

1. Install [BepInExPack for Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/).
2. Drop `ImmortalPortals.dll` into `BepInEx/plugins/ImmortalPortals/`.

## Multiplayer

Damage is decided by the object's owner (host / dedicated server). Install the mod on the server to guarantee protection; clients should install it too so every player sees the same behavior.

## Build

```powershell
cd ImmortalPortals
dotnet build .\ImmortalPortals.csproj -c Release
```

Output: `BepInEx/plugins/ImmortalPortals/ImmortalPortals.dll` (output path is preconfigured).

## Repository

https://github.com/1264600905/valheim-immortal-portals

---

## 简体中文

一个小型 Valheim 模组：原版两种传送门（木门 / 石门）免疫**一切伤害**——怪物、玩家、火焰、雪灾、火山灰与支撑坍塌都无法再摧毁它们；建造锤拆除照常可用。

- **两种原版传送门全覆盖**：`portal_wood`（传送门）与 `portal_stone`（石制传送门）。
- **免疫所有伤害路径**：
  - 怪物与玩家攻击、投射物、火焰、AoE 爆炸；
  - 环境磨损：无支撑坍塌、深北雪灾、火山灰与岩浆、持久事件。
- **建造锤仍可拆除**：拆除走独立代码路径，随时可以拆掉传送门并回收材料。
- **老存档立即生效**：不改血量、不写存档数据。
- **模组传送门变体**（使用原版 `TeleportWorld` 组件）同样受保护，可配置。

**原理**：建筑受到的所有伤害最终都汇聚到 `WearNTear.ApplyDamage`（攻击伤害经 `RPC_Damage`，环境磨损经 `UpdateWear`）。本模组只 Harmony 拦截这一个方法，对传送门直接跳过；建造锤拆除走 `WearNTear.Remove` → `RPC_Remove` → `Destroy`，不经过该方法，因此不受影响。

| 配置项 | 默认 | 说明 |
|---|---|---|
| Enabled | true | 总开关 |
| ProtectWoodPortal | true | 保护木制传送门 |
| ProtectStonePortal | true | 保护石制传送门 |
| ProtectOtherPortals | true | 保护其他使用原版 `TeleportWorld` 组件的传送门变体 |
| DebugLog | false | 输出被拦截的伤害日志（排查用） |

**联机**：伤害由对象归属者（主机 / 专用服务器）判定，服务端安装才能保证保护生效；客户端也建议安装，表现一致。

---

## 繁體中文

一個小型 Valheim 模組：原版兩種傳送門（木門 / 石門）免疫**一切傷害**——怪物、玩家、火焰、雪災、火山灰與支撐坍塌都無法再摧毀它們；建造錘拆除照常可用。

- **兩種原版傳送門全覆蓋**：`portal_wood`（傳送門）與 `portal_stone`（石製傳送門）。
- **免疫所有傷害路徑**：
  - 怪物與玩家攻擊、投射物、火焰、AoE 爆炸；
  - 環境磨損：無支撐坍塌、深北雪災、火山灰與岩漿、持久事件。
- **建造錘仍可拆除**：拆除走獨立程式路徑，隨時可以拆掉傳送門並回收材料。
- **舊存檔立即生效**：不改血量、不寫存檔資料。
- **模組傳送門變體**（使用原版 `TeleportWorld` 元件）同樣受保護，可設定。

**原理**：建築受到的所有傷害最終都匯聚到 `WearNTear.ApplyDamage`（攻擊傷害經 `RPC_Damage`，環境磨損經 `UpdateWear`）。本模組只 Harmony 攔截這一個方法，對傳送門直接跳過；建造錘拆除走 `WearNTear.Remove` → `RPC_Remove` → `Destroy`，不經過該方法，因此不受影響。

**連線**：傷害由物件歸屬者（主機 / 專用伺服器）判定，伺服器安裝才能保證保護生效；客戶端也建議安裝，表現一致。
