using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Fusion;
using GodOfArcher;

#if !UNITY_EDITOR && (UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS)
#error This sample doesn't support currently selected platform, please switch to Windows, Mac, Linux in Build Settings.
#endif

namespace GodOfArcher
{
    /// <summary>
    /// Runtime data structure to hold player information which must survive events like player death/disconnect.
    /// </summary>
    public struct PlayerData : INetworkStruct
    {
        [Networked, Capacity(24)]
        public string Nickname { get => default; set { } }
        public PlayerRef PlayerRef;
        public int Kills;
        public int Deaths;
        public int LastKillTick;
        public int StatisticPosition;
        public bool IsAlive;
        public bool IsConnected;
        public Team team;
    }

    public enum Team
    {
        Josen = 0,
        Chung = 1,
    }

    public enum EGameplayState
    {
        Skirmish = 0,
        Running = 1,
        Finished = 2,
    }

    /// <summary>
    /// Drives gameplay logic - state, timing, handles player connect/disconnect/spawn/despawn/death, calculates statistics.
    /// </summary>
    public class Gameplay : NetworkBehaviour
    {
        public GameUI GameUI;
        public Player josen_playerPrefab;
        public Player chung_playerPrefab;
        public Signal_Fire Signal_A;
        public Signal_Fire Signal_B;

        public float GameDuration = 180f;
        public float PlayerRespawnTime = 5f;
        public float DoubleDamageDuration = 30f;
        public int start_player_cnt = 2;
        public int all_round = 5;

        public Arrow_Containr _ArrowContainer;

        [Networked]
        [Capacity(32)]
        [HideInInspector]
        public NetworkDictionary<PlayerRef, PlayerData> PlayerData { get; }
        [Networked]
        [HideInInspector]
        public TickTimer RemainingTime { get; set; }
        [Networked]
        [HideInInspector]
        public EGameplayState State { get; set; }

        [Networked]
        public int josen_cnt { get; set; } = 0;
        [Networked]
        public int chung_cnt { get; set; } = 0;
        [Networked]
        public int josen_win { get; set; } = 0;
        [Networked]
        public int chung_win { get; set; } = 0;
        [Networked]
        public int Fire_A { get; set; } = 0;
        [Networked]
        public int Fire_B { get; set; } = 0;
        public bool DoubleDamageActive => State == EGameplayState.Running && RemainingTime.RemainingTime(Runner).GetValueOrDefault() < DoubleDamageDuration;

        private bool _isNicknameSent;
        private float _runningStateTime;
        private List<Player> _spawnedPlayers = new(16);
        private List<PlayerRef> _pendingPlayers = new(16);
        private List<PlayerData> _tempPlayerData = new(16);
        private List<Transform> _recentSpawnPoints = new(10);

        public void PlayerKilled(PlayerRef killerPlayerRef, PlayerRef victimPlayerRef, EWeaponType weaponType, bool isCriticalKill)
        {
            if (HasStateAuthority == false)
                return;

            Debug.Log("pre_Josen : " + josen_cnt);
            Debug.Log("pre_chung : " + chung_cnt);

            // Update statistics of the killer player.
            if (PlayerData.TryGet(killerPlayerRef, out PlayerData killerData))
            {
                killerData.Kills++;
                killerData.LastKillTick = Runner.Tick;
                PlayerData.Set(killerPlayerRef, killerData);
            }

            // Update statistics of the victim player.
            var playerData = PlayerData.Get(victimPlayerRef);
            playerData.Deaths++;
            playerData.IsAlive = false;
            PlayerData.Set(victimPlayerRef, playerData);

            // Inform all clients about the kill via RPC.
            RPC_PlayerKilled(killerPlayerRef, victimPlayerRef, weaponType, isCriticalKill);

            if (playerData.team == Team.Josen) josen_cnt--;
            if (playerData.team == Team.Chung) chung_cnt--;

            Debug.Log("post_Josen : " + josen_cnt);
            Debug.Log("post_chung : " + chung_cnt);


            //StartCoroutine(RespawnPlayer(victimPlayerRef, PlayerRespawnTime));

            //RecalculateStatisticPositions();
        }

        /*public override void Spawned()
        {
            if (Runner.Mode == SimulationModes.Server)
            {
                Application.targetFrameRate = TickRate.Resolve(Runner.Config.Simulation.TickRateSelection).Server;
            }

            if (Runner.GameMode == GameMode.Shared)
            {
                throw new System.NotSupportedException("This sample doesn't support Shared Mode, please start the game as Server, Host or Client.");
            }
        }*/

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority == false)
                return;

            // PlayerManager is a special helper class which iterates over list of active players (NetworkRunner.ActivePlayers) and call spawn/despawn callbacks on demand.
            PlayerManager.UpdatePlayerConnections(Runner, SpawnPlayer, DespawnPlayer);

            // Start gameplay when there are enough players connected.
            if (State == EGameplayState.Skirmish && PlayerData.Count >= start_player_cnt)
            {
                StartGameplay();
            }

            if (State == EGameplayState.Running)
            {
                _runningStateTime += Runner.DeltaTime;

                var sessionInfo = Runner.SessionInfo;

                // Hide the match after 60 seconds. Players won't be able to randomly connect to existing game and start new one instead.
                // Joining via party code should work.
                if (sessionInfo.IsVisible && (_runningStateTime > 60f || sessionInfo.PlayerCount >= sessionInfo.MaxPlayers))
                {
                    sessionInfo.IsVisible = false;
                }

                if (josen_cnt == 0) ChungRoundWinGameplay();
                if (chung_cnt == 0) JosenRoundWinGameplay();
                if (Fire_A == 3 || Fire_B == 3) JosenRoundWinGameplay();
                if (RemainingTime.Expired(Runner) && (Fire_A + Fire_B) == 0 )
                {
                    JosenRoundWinGameplay();
                }
                else if(RemainingTime.Expired(Runner) && (Fire_A + Fire_B) != 0)
                {
                    ChungRoundWinGameplay();
                }
            }
        }

        public override void Render()
        {
            if (Runner.Mode == SimulationModes.Server)
                return;

            // Every client must send its nickname to the server when the game is started.
            if (_isNicknameSent == false)
            {
                RPC_SetPlayerNickname(Runner.LocalPlayer, PlayerPrefs.GetString("Photon.Menu.Username"));
                _isNicknameSent = true;
            }
        }

        public void SetRemainingTime(float time, string name)
        {
            RemainingTime = TickTimer.CreateFromSeconds(Runner, time);
            if(name == "A")
            {
                Signal_B.SetInactive();
                Fire_A = 1;
            }
            if(name == "B")
            {
                Signal_A.SetInactive();
                Fire_B = 1;
            }
        }

        public void Fire_Extinguished()
        {
            Fire_A = 2;
            Fire_B = 2;
        }
        private void SpawnPlayer(PlayerRef playerRef)
        {
            if (PlayerData.TryGet(playerRef, out var playerData) == false)
            {
                playerData = new PlayerData();
                playerData.PlayerRef = playerRef;
                playerData.Nickname = playerRef.ToString();
                playerData.StatisticPosition = int.MaxValue;
                playerData.IsAlive = false;
                playerData.IsConnected = false;
                var randTeam = Random.Range(0,2);

                if (josen_cnt >= start_player_cnt / 2)
                {
                    randTeam = 1;
                }

                if (chung_cnt >= start_player_cnt / 2)
                {
                    randTeam = 0;
                }

                playerData.team = (Team)randTeam;
            }

            if (playerData.IsConnected == true)
                return;

            Debug.LogWarning($"{playerRef} connected.");

            playerData.IsConnected = true;
            playerData.IsAlive = true;

            PlayerData.Set(playerRef, playerData);

            var spawnPoint = GetSpawnPoint(playerData);

            Player PlayerPrefab = null;
            if(playerData.team == Team.Josen)
            {
                josen_cnt++;
                PlayerPrefab = josen_playerPrefab;
            }
            if(playerData.team == Team.Chung)
            {
                chung_cnt++;
                PlayerPrefab = chung_playerPrefab;
            }
            var player = Runner.Spawn(PlayerPrefab, spawnPoint.position, spawnPoint.rotation, playerRef);

            // Set player instance as PlayerObject so we can easily get it from other locations.
            Runner.SetPlayerObject(playerRef, player.Object);

            //RecalculateStatisticPositions();
        }

        private void DespawnPlayer(PlayerRef playerRef, Player player)
        {
            if (PlayerData.TryGet(playerRef, out var playerData) == true)
            {
                if (playerData.IsConnected == true)
                {
                    Debug.LogWarning($"{playerRef} disconnected.");
                }

                playerData.IsConnected = false;
                playerData.IsAlive = false;
                PlayerData.Set(playerRef, playerData);
            }

            Runner.Despawn(player.Object);

            RecalculateStatisticPositions();
        }

        private IEnumerator RespawnPlayer(PlayerRef playerRef, float delay)
        {
            if (delay > 0f)
                yield return new WaitForSecondsRealtime(delay);

            if (Runner == null)
                yield break;

            // Despawn old player object if it exists.
            var playerObject = Runner.GetPlayerObject(playerRef);
            if (playerObject != null)
            {
                Runner.Despawn(playerObject);
            }

            // Don't spawn the player for disconnected clients.
            if (PlayerData.TryGet(playerRef, out PlayerData playerData) == false || playerData.IsConnected == false)
                yield break;

            // Update player data.
            playerData.IsAlive = true;
            PlayerData.Set(playerRef, playerData);

            var spawnPoint = GetSpawnPoint(playerData);
            Player PlayerPrefab = null;
            if (playerData.team == Team.Josen)
            {
                PlayerPrefab = josen_playerPrefab;
            }
            if (playerData.team == Team.Chung)
            {
                PlayerPrefab = chung_playerPrefab;
            }
            
            var player = Runner.Spawn(PlayerPrefab, spawnPoint.position, spawnPoint.rotation, playerRef);

            // Set player instance as PlayerObject so we can easily get it from other locations.
            Runner.SetPlayerObject(playerRef, player.Object);

            /*Signal_A.SetActive();
            Signal_B.SetActive();
            Fire_A = 0;
            Fire_B = 0;*/
        }

        private Transform GetSpawnPoint(PlayerData playerData)
        {
            Transform spawnPoint = default;

            // Iterate over all spawn points in the scene.
            var spawnPoints = Runner.SimulationUnityScene.GetComponents<SpawnPoint>(false);
            for (int i = 0, offset = Random.Range(0, spawnPoints.Length); i < spawnPoints.Length; i++)
            {
                spawnPoint = default;
                if (playerData.team == Team.Josen && spawnPoints[(offset + i) % spawnPoints.Length].tag == "Josen_spawn") spawnPoint = spawnPoints[(offset + i) % spawnPoints.Length].transform;
                if (playerData.team == Team.Chung && spawnPoints[(offset + i) % spawnPoints.Length].tag == "Chung_spawn") spawnPoint = spawnPoints[(offset + i) % spawnPoints.Length].transform;
                if (spawnPoint == default) continue;

                if (_recentSpawnPoints.Contains(spawnPoint) == false)
                    break;
            }

            // Add spawn point to list of recently used spawn points.
            _recentSpawnPoints.Add(spawnPoint);

            /*// Ignore only last 3 spawn points.
            if (_recentSpawnPoints.Count > 1)
            {
                _recentSpawnPoints.RemoveAt(0);
            }*/

            return spawnPoint;
        }

        private void StartGameplay()
        {
            // Stop all respawn coroutines.
            StopAllCoroutines();
            _ArrowContainer.Reset_Arrow_Container();
            State = EGameplayState.Running;
            RemainingTime = TickTimer.CreateFromSeconds(Runner, GameDuration);
            _recentSpawnPoints.Clear();
            // Reset player data after skirmish and respawn players.
            foreach (var playerPair in PlayerData)
            {
                var data = playerPair.Value;

                data.Kills = 0;
                data.Deaths = 0;
                data.StatisticPosition = int.MaxValue;
                data.IsAlive = false;

                PlayerData.Set(data.PlayerRef, data);

                StartCoroutine(RespawnPlayer(data.PlayerRef, 0f));
            }
        }

        private void StopGameplay()
        {
            //RecalculateStatisticPositions();

            State = EGameplayState.Finished;
        }

        private void JosenRoundWinGameplay()
        {
            //RecalculateStatisticPositions();
            Debug.Log("Josen Win!");
            josen_win++;
            chung_cnt = start_player_cnt / 2;
            josen_cnt = start_player_cnt / 2;
            if (josen_win > all_round/2) StopGameplay();
            if(State != EGameplayState.Finished)StartGameplay();
        }

        private void ChungRoundWinGameplay()
        {
            //RecalculateStatisticPositions();
            Debug.Log("Chung Win!");
            chung_win++;
            josen_cnt = start_player_cnt / 2;
            chung_cnt = start_player_cnt / 2;
            if (chung_win > all_round / 2) StopGameplay();
            if (State != EGameplayState.Finished) StartGameplay();
        }



        private void RecalculateStatisticPositions()
        {
            if (State == EGameplayState.Finished)
                return;

            _tempPlayerData.Clear();

            foreach (var pair in PlayerData)
            {
                _tempPlayerData.Add(pair.Value);
            }

            _tempPlayerData.Sort((a, b) =>
            {
                if (a.Kills != b.Kills)
                    return b.Kills.CompareTo(a.Kills);

                return a.LastKillTick.CompareTo(b.LastKillTick);
            });

            for (int i = 0; i < _tempPlayerData.Count; i++)
            {
                var playerData = _tempPlayerData[i];
                playerData.StatisticPosition = playerData.Kills > 0 ? i + 1 : int.MaxValue;

                PlayerData.Set(playerData.PlayerRef, playerData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
        private void RPC_PlayerKilled(PlayerRef killerPlayerRef, PlayerRef victimPlayerRef, EWeaponType weaponType, bool isCriticalKill)
        {
            string killerNickname = "";
            string victimNickname = "";

            if (PlayerData.TryGet(killerPlayerRef, out PlayerData killerData))
            {
                killerNickname = killerData.Nickname;
            }

            if (PlayerData.TryGet(victimPlayerRef, out PlayerData victimData))
            {
                victimNickname = victimData.Nickname;
            }

            GameUI.GameplayView.KillFeed.ShowKill(killerNickname, victimNickname, weaponType, isCriticalKill);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        private void RPC_SetPlayerNickname(PlayerRef playerRef, string nickname)
        {
            var playerData = PlayerData.Get(playerRef);
            playerData.Nickname = nickname;
            PlayerData.Set(playerRef, playerData);
        }
    }
}
