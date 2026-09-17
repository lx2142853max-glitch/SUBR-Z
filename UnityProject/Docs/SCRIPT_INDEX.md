# Script index (sab C# files)

| Script | Namespace | Kaam |
|--------|-----------|------|
| `Core/GameBootstrap.cs` | SUBR.Core | Boot, services, first scene |
| `Core/GameSession.cs` | SUBR.Core | Cross-scene player prefs-like data |
| `Core/GameEvents.cs` | SUBR.Core | Global events (kill, zone, match) |
| `Core/ServiceLocator.cs` | SUBR.Core | Mini DI |
| `Core/GameIds.cs` | SUBR.Core | Tags / scene names |
| `Data/WeaponData.cs` | SUBR.Data | SO weapon stats |
| `Data/ItemData.cs` | SUBR.Data | SO items |
| `Data/MatchConfig.cs` | SUBR.Data | SO zone + match rules |
| `Player/PlayerController.cs` | SUBR.Player | Move / look / jump |
| `Player/Health.cs` | SUBR.Player | HP armor death |
| `Camera/TpsCameraFollow.cs` | SUBR.CameraSys | Shoulder camera |
| `Combat/IDamageable.cs` | SUBR.Combat | Damage interface |
| `Combat/DamageableHealthAdapter.cs` | SUBR.Combat | Health → IDamageable |
| `Combat/Hitbox.cs` | SUBR.Combat | Body/head colliders |
| `Weapons/WeaponController.cs` | SUBR.Weapons | Fire / reload / hitscan |
| `Inventory/InventorySystem.cs` | SUBR.Inventory | Slots + equip |
| `Inventory/ItemPickup.cs` | SUBR.Inventory | Trigger loot |
| `GameMode/SafeZone.cs` | SUBR.GameMode | Shrink + outside DoT |
| `GameMode/MatchDirector.cs` | SUBR.GameMode | Spawn, alive, win/lose |
| `AI/BotController.cs` | SUBR.AI | Chase / shoot bots |
| `Network/INetworkSession.cs` | SUBR.Network | Net abstraction |
| `Network/OfflineNetworkSession.cs` | SUBR.Network | Offline stub |
| `UI/HudPresenter.cs` | SUBR.UI | HUD binds |
| `UI/LobbyUI.cs` | SUBR.UI | Name + play button |
| `Utility/Billboard.cs` | SUBR.Utility | Face camera |
| `Utility/DebugOverlay.cs` | SUBR.Utility | F1 debug |
| `Editor/SubrEditorMenus.cs` | SUBR.EditorTools | Create SO menu |

**Language:** 100% **C#** (Unity scripting).  
**Tum Editor mein:** scenes, prefabs, models, NavMesh, UI canvas wiring, Android IL2CPP build.
