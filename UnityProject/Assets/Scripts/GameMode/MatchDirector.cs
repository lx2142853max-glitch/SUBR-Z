using System.Collections.Generic;
using SUBR.Core;
using SUBR.Data;
using SUBR.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SUBR.GameMode
{
    public enum MatchState
    {
        Idle,
        Warmup,
        Live,
        Ended
    }

    /// <summary>Match scene root controller: spawns, alive tracking, win/lose.</summary>
    public sealed class MatchDirector : MonoBehaviour
    {
        [SerializeField] MatchConfig config;
        [SerializeField] SafeZone safeZone;
        [SerializeField] Transform[] spawnPoints;
        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject botPrefab;
        [SerializeField] bool spawnOnStart = true;

        public MatchState State { get; private set; } = MatchState.Idle;
        public int AliveCount { get; private set; }

        readonly List<Health> _actors = new List<Health>();
        Health _localPlayer;
        float _warmupLeft;
        float _matchTime;
        int _localKills;

        void OnEnable()
        {
            GameEvents.OnPlayerKilled += HandleKill;
        }

        void OnDisable()
        {
            GameEvents.OnPlayerKilled -= HandleKill;
        }

        void Start()
        {
            if (spawnOnStart) BeginMatch();
        }

        public void BeginMatch()
        {
            if (config == null)
            {
                Debug.LogError("[SUBR] MatchConfig missing on MatchDirector");
                return;
            }

            GameSession.Instance.ResetMatchStats();
            _localKills = 0;
            _matchTime = 0f;
            State = MatchState.Warmup;
            _warmupLeft = config.WarmupSeconds;
            GameEvents.RaiseMatchStarting();

            SpawnAll();
            if (safeZone) safeZone.Begin(config);

            RecountAlive();
            Debug.Log($"[SUBR] Warmup {_warmupLeft}s, actors={_actors.Count}");
        }

        void Update()
        {
            if (State == MatchState.Warmup)
            {
                _warmupLeft -= Time.deltaTime;
                if (_warmupLeft <= 0f)
                {
                    State = MatchState.Live;
                    GameEvents.RaiseMatchStarted();
                    GameEvents.Toast("Match live!");
                }
                return;
            }

            if (State != MatchState.Live) return;
            _matchTime += Time.deltaTime;
            CheckEndConditions();
        }

        void SpawnAll()
        {
            _actors.Clear();
            int idx = 0;

            if (playerPrefab != null)
            {
                var p = Instantiate(playerPrefab, NextSpawn(ref idx), Quaternion.identity);
                _localPlayer = p.GetComponentInChildren<Health>();
                if (_localPlayer) _actors.Add(_localPlayer);
            }
            else
            {
                _localPlayer = FindObjectOfType<PlayerController>()?.GetComponent<Health>();
                if (_localPlayer) _actors.Add(_localPlayer);
            }

            int bots = GameSession.Instance.OfflineBots
                ? Mathf.Max(config.BotFillCount, GameSession.Instance.TargetBotCount)
                : 0;

            if (botPrefab != null)
            {
                for (int i = 0; i < bots; i++)
                {
                    var b = Instantiate(botPrefab, NextSpawn(ref idx), Quaternion.identity);
                    var h = b.GetComponentInChildren<Health>();
                    if (h != null)
                    {
                        h.ConfigureId(1000 + i);
                        _actors.Add(h);
                    }
                    var bot = b.GetComponentInChildren<AI.BotController>();
                    bot?.BindLocalPlayer(_localPlayer != null ? _localPlayer.transform : null);
                }
            }
        }

        Vector3 NextSpawn(ref int idx)
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return transform.position + Random.insideUnitSphere * 5f;

            var t = spawnPoints[idx % spawnPoints.Length];
            idx++;
            return t != null ? t.position : transform.position;
        }

        void HandleKill(int killer, int victim)
        {
            if (_localPlayer != null && killer == _localPlayer.ActorId)
            {
                _localKills++;
                GameSession.Instance.LastKills = _localKills;
            }
            RecountAlive();
        }

        void RecountAlive()
        {
            int n = 0;
            for (int i = 0; i < _actors.Count; i++)
                if (_actors[i] != null && !_actors[i].IsDead) n++;
            AliveCount = n;
            GameEvents.RaiseAliveCount(n);
        }

        void CheckEndConditions()
        {
            if (_localPlayer != null && _localPlayer.IsDead)
            {
                EndMatch(false, AliveCount + 1, "Eliminated");
                return;
            }

            if (AliveCount <= 1 && _localPlayer != null && !_localPlayer.IsDead)
            {
                EndMatch(true, 1, "Winner");
            }
        }

        void EndMatch(bool victory, int placement, string reason)
        {
            if (State == MatchState.Ended) return;
            State = MatchState.Ended;
            safeZone?.StopZone();

            var result = new MatchResult
            {
                Victory = victory,
                Placement = placement,
                Kills = _localKills,
                SurvivalSeconds = _matchTime,
                Reason = reason
            };
            GameSession.Instance.LastPlacement = placement;
            GameSession.Instance.LastKills = _localKills;
            GameEvents.RaiseMatchEnded(result);
            Debug.Log($"[SUBR] Match ended: {reason} place={placement} kills={_localKills}");
        }

        public void ReturnToLobby()
        {
            SceneManager.LoadScene(GameIds.SceneLobby);
        }
    }
}
