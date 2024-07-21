using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner networkRunner; // Khai báo NetworkRunner để quản lý kết nối mạng
    [SerializeField] NetworkPrefabRef networkPrefabRef; // Tham chiếu đến prefab mạng sẽ được spawn

    private Dictionary<PlayerRef, NetworkObject> spawnCharacter = new Dictionary<PlayerRef, NetworkObject>(); // Từ điển để lưu trữ các đối tượng mạng được spawn

    private bool _mouseButtonO;
    private bool _mouseButton1;
    // Phương thức bất đồng bộ để bắt đầu trò chơi với chế độ đã chọn
    async void GameStart(GameMode gameMode)
    {
        // Tạo và cấu hình NetworkRunner, thành phần quản lý mạng
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        gameObject.AddComponent<RunnerSimulatePhysics3D>();
        networkRunner.ProvideInput = true; // Cho phép NetworkRunner cung cấp dữ liệu đầu vào

        // Tạo tham chiếu đến cảnh hiện tại dựa trên chỉ số của cảnh
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo(); // Tạo đối tượng để lưu thông tin cảnh

        // Nếu tham chiếu cảnh hợp lệ, thêm cảnh vào NetworkSceneInfo
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive); // Tải cảnh thêm vào chế độ Additive
        }

        // Khởi động trò chơi với các tham số cấu hình
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = gameMode, // Xác định chế độ trò chơi (Host, Client, Server)
            SessionName = "TestGameNestedMango", // Đặt tên cho phiên trò chơi
            Scene = scene, // Chỉ định cảnh cần tải
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>() // Thêm NetworkSceneManagerDefault để quản lý cảnh
        });
    }

    // Phương thức OnGUI để tạo các nút bấm bắt đầu trò chơi
    private void OnGUI()
    {
        if (networkRunner == null)
        {
            // Nút bấm để bắt đầu trò chơi ở chế độ Host
            if (GUI.Button(new Rect(0, 0, 200, 100), "Host"))
            {
                GameStart(GameMode.Host);
            }
            // Nút bấm để tham gia trò chơi ở chế độ Client
            if (GUI.Button(new Rect(0, 120, 200, 100), "Join"))
            {
                GameStart(GameMode.Client);
            }
        }
    }

    // Được gọi khi client kết nối thành công đến server
    public void OnConnectedToServer(NetworkRunner runner)
    {
        // Xử lý khi kết nối thành công, ví dụ: cập nhật giao diện người dùng hoặc bắt đầu đồng bộ hóa dữ liệu
    }

    // Được gọi khi kết nối đến server không thành công
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        // Xử lý khi kết nối thất bại, ví dụ: hiển thị thông báo lỗi cho người chơi
    }

    // Được gọi khi có yêu cầu kết nối đến server từ một client khác
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // Xử lý yêu cầu kết nối và quyết định chấp nhận hoặc từ chối
    }

    // Được gọi khi server nhận được phản hồi từ quá trình xác thực tùy chỉnh
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // Xử lý phản hồi từ các hệ thống xác thực tùy chỉnh nếu có
    }

    // Được gọi khi client bị ngắt kết nối khỏi server
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        // Xử lý khi kết nối bị mất, ví dụ: cập nhật giao diện người dùng hoặc thực hiện dọn dẹp
    }

    // Được gọi khi cần thực hiện việc chuyển giao quyền quản lý trò chơi từ host hiện tại sang một host mới
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // Xử lý chuyển giao quyền quản lý trò chơi để đảm bảo trò chơi tiếp tục hoạt động
    }

    // Được gọi khi một client gửi dữ liệu đầu vào (input) đến server
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Xử lý dữ liệu đầu vào từ người chơi, như di chuyển hoặc hành động trong trò chơi
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
        {
            data.direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            data.direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            data.direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            data.direction += Vector3.right;
        }
        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButtonO);
        _mouseButtonO = false;

        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, _mouseButton1);
        _mouseButton1 = false;

        input.Set(data);
    }

    // Được gọi khi dữ liệu đầu vào của một người chơi bị thiếu hoặc không nhận được
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // Xử lý tình huống khi dữ liệu đầu vào của một client không được gửi đúng cách
    }

    // Được gọi khi một đối tượng (object) vào khu vực quan sát của một người chơi (Area of Interest)
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // Xử lý khi một đối tượng mới vào khu vực quan sát của người chơi
    }

    // Được gọi khi một đối tượng (object) rời khỏi khu vực quan sát của một người chơi
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // Xử lý khi một đối tượng không còn nằm trong khu vực quan sát của người chơi
    }

    // Được gọi khi một người chơi mới tham gia vào trò chơi
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Xử lý khi một người chơi mới gia nhập, ví dụ: thông báo cho các người chơi khác
        if (runner.IsServer)
        {
            // Đặt vị trí spawn của người chơi dựa trên số lượng người chơi hiện tại
            Vector3 playerPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1f, 0f);

            // Spawn đối tượng mạng cho người chơi mới
            NetworkObject networkObject = runner.Spawn(networkPrefabRef, playerPosition, Quaternion.identity, player);

            // Thêm đối tượng mạng vào từ điển để quản lý
            spawnCharacter.Add(player, networkObject);
        }
    }

    // Được gọi khi một người chơi rời khỏi trò chơi
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Xử lý khi một người chơi không còn trong trò chơi, ví dụ: cập nhật giao diện người dùng
        if (spawnCharacter.TryGetValue(player, out NetworkObject networkObject))
        {
            // Hủy đối tượng mạng của người chơi đã rời khỏi
            runner.Despawn(networkObject);

            // Loại bỏ người chơi khỏi từ điển quản lý
            spawnCharacter.Remove(player);
        }
    }

    // Được gọi để thông báo về tiến độ của dữ liệu đáng tin cậy (reliable data) từ một người chơi
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        // Xử lý tiến độ của dữ liệu đáng tin cậy, ví dụ: cập nhật giao diện hoặc trạng thái
    }

    // Được gọi khi dữ liệu đáng tin cậy (reliable data) được nhận từ một người chơi
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        // Xử lý dữ liệu đáng tin cậy nhận được từ mạng
    }

    // Được gọi khi việc tải cảnh (scene) hoàn tất
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // Xử lý sau khi cảnh đã được tải xong, ví dụ: khởi tạo các đối tượng hoặc cấu hình trò chơi
    }

    // Được gọi khi bắt đầu tải cảnh (scene)
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        // Xử lý khi bắt đầu tải cảnh, ví dụ: thực hiện các bước chuẩn bị trước khi cảnh được tải
    }

    // Được gọi khi danh sách các phiên trò chơi (sessions) được cập nhật
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // Xử lý khi danh sách các phiên trò chơi thay đổi, ví dụ: cập nhật giao diện người dùng
    }

    // Được gọi khi network runner bị tắt
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        // Xử lý khi ứng dụng hoặc trò chơi tắt, ví dụ: lưu trạng thái trò chơi hoặc dọn dẹp tài nguyên
    }

    // Được gọi khi nhận được tin nhắn mô phỏng từ người dùng
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // Xử lý các thông điệp mô phỏng tùy chỉnh gửi qua mạng
    }
    private void Update()
    {
        _mouseButtonO = _mouseButtonO | Input.GetMouseButtonDown(0);
        _mouseButton1 = _mouseButton1 | Input.GetMouseButtonDown(1);
    }
}
