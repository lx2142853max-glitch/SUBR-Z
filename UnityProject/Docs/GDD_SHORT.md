# Short GDD — SUBR-like prototype

## Fantasy
Third-person / free-roam battle: drop in, loot, shrink zone, last squad/player wins.

## Modes (code supports flags)
| Mode | Description |
|------|-------------|
| Solo | 1 life, last alive |
| Duo/Squad | shared wipe (stub team id) |
| Offline practice | bots only |

## Loop
1. Lobby loadout  
2. Match start → spawn  
3. Loot weapons / heals  
4. Zone phases damage outside  
5. Kills → alive count  
6. Win / defeat → results → lobby  

## Systems priority
P0: move, camera, shoot, health, zone, match state, bots  
P1: inventory, pickups, HUD  
P2: network, progression, ads/IAP hooks  

## Non-goals (v1 code)
- Full anti-cheat  
- Ranked backend  
- Battle pass  
- Voice chat  

## Metrics to log later
match_id, placement, kills, damage, survival_time, device
