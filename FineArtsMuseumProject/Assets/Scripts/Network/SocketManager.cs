using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Camera;
using Newtonsoft.Json.Linq;
using Player;
using UnityEngine;

namespace Network
{
    public class SocketManager : MonoBehaviour
    {
        [Header("Connection Settings")]
        [SerializeField] private string serverIP = "127.0.0.1";
        [SerializeField] private int serverPort = 5000;

        [Header("Status")]
        [SerializeField] private bool isConnected;
        [SerializeField] private string connectionStatus = "Disconnected";

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private Thread _receiveThread;
        private bool _shouldReceive;

        // Event để thông báo khi nhận được command
        public event Action<string, string> OnCommandReceived;

        /// <summary>
        /// Khởi tạo kết nối socket với server
        /// </summary>
        /// <param name="ip">Địa chỉ IP server</param>
        /// <param name="port">Port server</param>
        public void InitSocket(string ip, int port)
        {
            serverIP = ip;
            serverPort = port;

            try
            {
                // Đóng kết nối cũ nếu có
                Disconnect();

                // Tạo kết nối mới
                _tcpClient = new TcpClient();
                _tcpClient.Connect(serverIP, serverPort);
                _networkStream = _tcpClient.GetStream();

                isConnected = true;
                connectionStatus = "Connected";

                Debug.Log($"Connected to server at {serverIP}:{serverPort}");

                // Bắt đầu thread nhận dữ liệu
                _shouldReceive = true;
                _receiveThread = new Thread(ReceiveData)
                {
                    IsBackground = true
                };

                UnityMainThreadDispatcher.Instance.Initialize();
                
                _receiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.LogError($"Connection failed: {e.Message}");
                connectionStatus = $"Connection failed: {e.Message}";
                isConnected = false;
            }
        }

        /// <summary>
        /// Thread nhận dữ liệu từ server
        /// </summary>
        private void ReceiveData()
        {
            byte[] buffer = new byte[1024];
            StringBuilder stringBuilder = new StringBuilder();

            while (_shouldReceive && _tcpClient != null && _tcpClient.Connected)
            {
                try
                {
                    // Kiểm tra nếu có dữ liệu để đọc
                    if (_networkStream != null && _networkStream.DataAvailable)
                    {
                        int bytesRead = _networkStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            stringBuilder.Append(receivedData);

                            // Xử lý các message đầy đủ (giả sử mỗi message kết thúc bằng \n)
                            string allData = stringBuilder.ToString();
                            string[] messages = allData.Split('\n');

                            // Giữ lại phần chưa xử lý hết
                            stringBuilder.Clear();
                            if (allData.EndsWith("\n") == false && messages.Length > 0)
                            {
                                stringBuilder.Append(messages[messages.Length - 1]);
                            }

                            // Xử lý từng message đầy đủ
                            for (int i = 0; i < messages.Length - 1; i++)
                            {
                                if (!string.IsNullOrEmpty(messages[i]))
                                {
                                    ProcessMessage(messages[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(10); // Giảm CPU usage
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error receiving data: {e.Message}");
                    _shouldReceive = false;
                }
            }
        }

        /// <summary>
        /// Xử lý message JSON nhận được
        /// </summary>
        private void ProcessMessage(string jsonMessage)
        {
            try
            {
                Debug.Log($"Received JSON: {jsonMessage}");

                // Parse JSON message
                JObject jsonObject = JObject.Parse(jsonMessage);

                // Lấy command và value
                string command = jsonObject["command"]?.ToString();
                string value = jsonObject["value"]?.ToString();

                if (!string.IsNullOrEmpty(command))
                {
                    // Gọi event trên main thread
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        HandleCommand(command, value);
                    });

                    // Hoặc có thể gọi trực tiếp nếu không cần chuyển về main thread
                    // HandleCommand(command, value);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing JSON: {e.Message}\nMessage: {jsonMessage}");
            }
        }

        /// <summary>
        /// Xử lý command bằng switch case
        /// </summary>
        private void HandleCommand(string command, string value)
        {
            switch (command)
            {
                case "UPDATE_FLOOR":
                    OnUpdateFloor(value);
                    break;

                case "UPDATE_POSITION":
                    OnUpdatePosition(value);
                    break;

                case "UPDATE_PLAYER_COUNT":
                    OnUpdatePlayerCount(value);
                    break;

                case "SEND_MESSAGE":
                    OnReceiveMessage(value);
                    break;

                case "UPDATE_SCORE":
                    OnUpdateScore(value);
                    break;

                // Thêm các command khác tại đây
                default:
                    Debug.LogWarning($"Unknown command: {command}");
                    break;
            }

            // Gọi event để các component khác có thể lắng nghe
            OnCommandReceived?.Invoke(command, value);
        }

        #region Command Handlers
        private void OnUpdateFloor(string value)
        {
            if(!int.TryParse(value, out var floor)) return;
            CharacterManager.Instance.SetFloorForCharacter(floor);
            CameraManager.Instance.SetCameraRotationByFloor(floor);
        }

        private void OnUpdatePosition(string value)
        {
            // Ví dụ: value = "10.5,20.3,5.2"
            string[] coords = value.Split(',');
            if (coords.Length == 3)
            {
                float x = float.Parse(coords[0]);
                float y = float.Parse(coords[1]);
                float z = float.Parse(coords[2]);
                Debug.Log($"Update Position: ({x}, {y}, {z})");
            }
        }

        private void OnUpdatePlayerCount(string value)
        {
            int playerCount = int.Parse(value);
            Debug.Log($"Player Count Updated: {playerCount}");
        }

        private void OnReceiveMessage(string value)
        {
            Debug.Log($"Message Received: {value}");
        }

        private void OnUpdateScore(string value)
        {
            string[] parts = value.Split(':');
            if (parts.Length == 2)
            {
                string playerId = parts[0];
                int score = int.Parse(parts[1]);
                Debug.Log($"Player {playerId} Score: {score}");
            }
        }
        #endregion

        /// <summary>
        /// Gửi dữ liệu đến server
        /// </summary>
        public void SendData(string data)
        {
            if (!isConnected || _networkStream == null)
            {
                Debug.LogWarning("Not connected to server");
                return;
            }

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data + "\n");
                _networkStream.Write(bytes, 0, bytes.Length);
                _networkStream.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error sending data: {e.Message}");
                Disconnect();
            }
        }

        /// <summary>
        /// Gửi JSON data đến server
        /// </summary>
        public void SendJson(JObject jsonObject)
        {
            string jsonString = jsonObject.ToString(Newtonsoft.Json.Formatting.None);
            SendData(jsonString);
        }

        /// <summary>
        /// Ngắt kết nối
        /// </summary>
        private void Disconnect()
        {
            _shouldReceive = false;

            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Join(1000);
            }

            if (_networkStream != null)
            {
                _networkStream.Close();
                _networkStream = null;
            }

            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }

            isConnected = false;
            connectionStatus = "Disconnected";
            Debug.Log("Disconnected from server");
        }

        void OnDestroy()
        {
            Disconnect();
        }

        void OnApplicationQuit()
        {
            Disconnect();
        }

        #region Helper Methods
        public bool IsConnected()
        {
            return isConnected && _tcpClient != null && _tcpClient.Connected;
        }

        public string GetConnectionStatus()
        {
            return connectionStatus;
        }

        // Ví dụ method để gửi command
        public void SendUpdateFloorCommand(int floor, int room)
        {
            JObject json = new JObject
            {
                ["command"] = "UPDATE_FLOOR",
                ["value"] = $"{floor}:{room}"
            };
            SendJson(json);
        }
        #endregion
    }
}