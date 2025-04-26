using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    [Networked] public NetworkBool IsGameStarted { get; set; }
    [Networked] public int PlayerCount { get; set; }
    [Networked] private NetworkDictionary<PlayerRef, NetworkBool> PlayerReadyStates => default;

    public const int MAX_PLAYERS = 2;

    public delegate void GameStartDelegate();
    public event GameStartDelegate OnGameStart;

    public override void Spawned()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("GameManager spawned and initialized");
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            PlayerCount++;
            PlayerReadyStates.Set(player, false);
            Debug.Log($"Player {player} joined. Total players: {PlayerCount}. Ready states: {GetReadyStatesString()}");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            PlayerCount--;
            PlayerReadyStates.Remove(player);
            Debug.Log($"Player {player} left. Total players: {PlayerCount}. Ready states: {GetReadyStatesString()}");

            if (IsGameStarted && PlayerCount < MAX_PLAYERS)
            {
                EndGame();
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerReady(PlayerRef player)
    {
        Debug.Log($"RPC_PlayerReady called for player {player}");
        if (PlayerReadyStates.ContainsKey(player))
        {
            PlayerReadyStates.Set(player, true);
            Debug.Log($"Player {player} is ready. Ready states: {GetReadyStatesString()}");
            CheckAllPlayersReady();
        }
        else
        {
            Debug.LogWarning($"Attempted to set ready state for non-existent player: {player}");
        }
    }

    private void CheckAllPlayersReady()
    {
        if (HasStateAuthority)
        {
            bool allReady = PlayerReadyStates.Count == MAX_PLAYERS &&
                            !PlayerReadyStates.Any(kvp => !kvp.Value);

            Debug.Log($"Checking all players ready. All ready: {allReady}, Player count: {PlayerReadyStates.Count}, Ready count: {PlayerReadyStates.Count(kvp => kvp.Value)}");

            if (allReady)
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        if (!IsGameStarted)
        {
            IsGameStarted = true;
            Debug.Log("All players are ready. Game is starting!");

            // 각 플레이어의 상태를 "Playing"으로 변경
            foreach (var kvp in PlayerReadyStates)
            {
                PlayerRef player = kvp.Key;
                Player playerObject = GetPlayerObject(player);
                if (playerObject != null)
                {
                  //  playerObject.SetPlayerState("Playing");
                }
            }

            RPC_NotifyGameStart();
        }
    }

    private Player GetPlayerObject(PlayerRef player)
    {
        if (NetworkManager.Instance._spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            return networkObject.GetComponent<Player>();
        }
        return null;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyGameStart()
    {
        Debug.Log("Game started notification received");
        OnGameStart?.Invoke();
    }

    private void EndGame()
    {
        if (HasStateAuthority)
        {
            IsGameStarted = false;
            foreach (var kvp in PlayerReadyStates)
            {
                PlayerReadyStates.Set(kvp.Key, false);
            }
            Debug.Log("Game ended.");
        }
    }

    public string GetPlayerState(PlayerRef player)
    {
        if (PlayerReadyStates.TryGet(player, out NetworkBool isReady))
        {
            return isReady ? "Ready" : "Waiting";
        }
        return "Unknown";
    }

    private string GetReadyStatesString()
    {
        return string.Join(", ", PlayerReadyStates.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }
}