using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


// NetworkManager 클래스
public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;               // 플레이어 프리펩
    [SerializeField] private NetworkPrefabRef _gameManagerPrefab;          // 게임 매니져 프리펩
    private NetworkRunner _runner;                                         // 네트워크 관련 기능 제공
    private NetworkInputHandler _inputHandler;                             // 네트워크 입력을 처리하는 기능을 담당
    private string _roomName = "TestRoom";                                 // 기본 방 이름
    [SerializeField] private string _gameSceneName = "GameScene";          // 게임 씬 이름

    // 플레이어 참조(PlayerRef)와 네트워크 객체(NetworkObject)의 관계를 나타내는 딕셔너리를 선언하고 초기화
    public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // 싱그톤 처리
    private static NetworkManager _instance;                                                                                
    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)                                      // _instance 존재하지 않으면
            {
                _instance = FindObjectOfType<NetworkManager>();         // _instance를 찾아오고
                if (_instance == null)                                  // _instance 존재 하지 않으면
                {
                    GameObject newManager = new GameObject("NetworkManager");      
                    _instance = newManager.AddComponent<NetworkManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)                                          // 만약 인스턴스가 없으면
        {
            _instance = this;                                           // 인스턴스를 현제 오브젝트로 지정하고
            DontDestroyOnLoad(gameObject);                              // 씬 전환 시에도 NetworkManager가 유지
        }
        else if (_instance != this)                                     // 만약 인스턴스가 존재한다면
        {
            Destroy(gameObject);                                        // 기존 인스턴스가 있으면 새로 생성된 인스턴스를 파괴
        }

        _inputHandler = gameObject.AddComponent<NetworkInputHandler>(); 
    }
    private void OnGUI()
    {
        if (_runner == null)                                            // 만약 _runner 존재하지 않으면
        {
            // 방 이름 입력 필드
            // 사각형 형태로 좌측상단 좌표(x = 10, y = 10)에 (x = 300 y = 300) 크기로 생성
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));            
            GUILayout.Label("방 번호:");
            _roomName = GUILayout.TextField(_roomName);

            // 호스트 버튼
            if (GUILayout.Button("호스트"))                            
            {
                StartGame(GameMode.Host);
            }
            // 참가 버튼
            if (GUILayout.Button("참가"))
            {
                StartGame(GameMode.Client);
            }
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GameManager.Instance != null && GameManager.Instance.IsGameStarted)
            {
                GUILayout.Label("게임 진행 중");
            }
            else
            {
                if (GUILayout.Button("Ready"))
                {
                    Debug.Log("Ready button clicked");
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.RPC_PlayerReady(_runner.LocalPlayer);
                    }
                    else
                    {
                        Debug.LogError("GameManager instance is null");
                    }
                }
                if (GameManager.Instance != null)
                {
                    GUILayout.Label($"현재 플레이어 수: {GameManager.Instance.PlayerCount}");
                    GUILayout.Label($"준비 상태: {GameManager.Instance.GetPlayerState(_runner.LocalPlayer)}");
                }
            }
            GUILayout.EndArea();
        }
    }

    async void StartGame(GameMode mode)
    {
        // Fusion 러너를 생성하고 사용자 입력을 제공할 것임을 알림
        _runner = gameObject.AddComponent<NetworkRunner>();

        // 만약 false 일 경우 플레이어는 캐릭터를 조작 할수 없다
        _runner.ProvideInput = true;

        // 게임 씬의 빌드 인덱스 찾기
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(_gameSceneName);
        if (sceneIndex == -1)
        {
            Debug.LogError($"{_gameSceneName}을(를) 빌드 설정에서 찾을 수 없습니다!");
            return;
        }

        // Load the target scene
        var gameScene = SceneRef.FromIndex(sceneIndex);

        // Check if the scene was found
        if (gameScene == SceneRef.None)
        {
            Debug.LogError("GameScene could not be found in the available scenes!");
            return;
        }

        // 세션 시작 또는 참가
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = _roomName, // 사용자가 입력한 방 이름 사용
            Scene = gameScene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = GetNextSpawnPosition();
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            if (networkPlayerObject != null)
            {
                _spawnedCharacters.Add(player, networkPlayerObject);
            }
            else
            {
                Debug.LogError("Failed to spawn player object.");
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerJoined(player);
        }

        Debug.Log($"플레이어 참가: {player}");
    }


    private void OnGameStart()
    {
        Debug.Log("Game is starting! Performing necessary actions...");
        // 여기에 게임 시작 시 필요한 추가 로직을 구현하세요.
        // 예: 플레이어 위치 재설정, 게임 오브젝트 활성화 등
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
        GameManager.Instance.PlayerLeft(player);
        Debug.Log($"플레이어 퇴장: {player}");
    }

    private Vector3 GetNextSpawnPosition()
    {
        // 간단한 예: 플레이어 수에 따라 원형으로 배치
        float angle = _spawnedCharacters.Count * (360f / 4); // 최대 8명의 플레이어 가정
        float radius = 5f; // 스폰 반경
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        return new Vector3(x, 1, z);
    }



    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_inputHandler.GetNetworkInput());
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"네트워크 셧다운: {shutdownReason}");
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("서버에 연결됨");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"서버와의 연결이 끊김: {reason}");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"연결 실패: {reason}");
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("씬 로드 완료");
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("씬 로드 시작");
    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"오브젝트가 AOI를 벗어남: {obj.Id}");
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"오브젝트가 AOI에 들어옴: {obj.Id}");
    }
}