using System;
using System.Collections.Generic;
using Camera;
using Newtonsoft.Json.Linq;
using Player;
using SocketIOClient;
using UnityEngine;

namespace Network
{
    public class SocketManager : MonoBehaviour
    {
        [Header("Connection Settings")]
        [SerializeField] private string serverIP = "http://127.0.0.1:3000"; // <-- Socket.IO dùng http

        [Header("Status")]
        [SerializeField] private bool isConnected;
        [SerializeField] private string connectionStatus = "Disconnected";

        private SocketIO _client;

        public event Action<string, string> OnCommandReceived;

        // Init socket
        public async void InitSocket(string serverIPInput, string playerId)
        {
            try
            {
                UnityMainThreadDispatcher.Instance.Initialize();
                Disconnect();
                
                serverIP = serverIPInput;
                //serverIP = "https://streaming-13.daiquocviet.vn:42003";

                _client = new SocketIO(serverIP, new SocketIOOptions()
                {
                    Reconnection = true,
                    Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
                    Query = new Dictionary<string, string>
                    {
                        {"clientId", playerId}
                    }
                });

                _client.OnConnected += (sender, e) =>
                {
                    isConnected = true;
                    connectionStatus = "Connected";
                    Debug.Log("✅ Connected to Socket.IO server");
                };

                _client.OnDisconnected += (sender, e) =>
                {
                    isConnected = false;
                    connectionStatus = "Disconnected";
                    Debug.Log("❌ Disconnected from server");
                };

                // nhận message từ server
                _client.On("update-unity", response =>
                {
                    try
                    {
                        // Đây là cách đúng – thay cho Args
                        var jsonString = response.GetValue<string>();

                        ProcessMessage(jsonString);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Read error: {e.Message}");
                    }
                });

                await _client.ConnectAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Connection failed: {e.Message}");
                connectionStatus = $"Connection failed: {e.Message}";
            }
        }

        /// xử lý JSON
        private void ProcessMessage(string jsonMessage)
        {
            try
            {
                Debug.Log($"Received JSON: {jsonMessage}");

                JObject jsonObject = JObject.Parse(jsonMessage);

                string command = jsonObject["command"]?.ToString();
                string value = jsonObject["value"]?.ToString();

                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    HandleCommand(command, value);
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON Error: {e.Message}\nMessage: {jsonMessage}");
            }
        }

        private void HandleCommand(string command, string value)
        {
            switch (command)
            {
                case "MOLINE":
                    OpenLine(value);
                    break;
                case "CHUYENNGONNGU":
                    TranferLanguage(value);
                    break;
                default:
                    Debug.LogWarning($"Unknown command: {command}");
                    break;
            }

            OnCommandReceived?.Invoke(command, value);
        }

        #region Command Handlers

        private void OnUpdateFloor(string value)
        {
            if (!int.TryParse(value, out var floor)) return;
            CharacterManager.Instance.SetFloorForCharacter(floor);
            CameraManager.Instance.SetCameraRotationByFloor(floor);
        }

        private void OpenLine(string value)
        {
            if (!int.TryParse(value, out var floor)) return;
            //OpenLine(floor);
        }
        
        private void TranferLanguage(string value)
        {
            if(value != "vi" && value != "en") return;
            AudioSubtitleManager.Instance.ChangeLanguage(value);
        }

        #endregion

        // Gửi message
        public async void SendData(string data)
        {
            if (!isConnected || _client == null)
            {
                Debug.LogWarning("Not connected");
                return;
            }

            await _client.EmitAsync("message", data);
        }

        public void SendJson(JObject jsonObject)
        {
            SendData(jsonObject.ToString());
        }

        public async void Disconnect()
        {
            if (_client != null)
            {
                await _client.DisconnectAsync();
                _client.Dispose();
                _client = null;
            }

            isConnected = false;
            connectionStatus = "Disconnected";
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        public bool IsConnected() => isConnected;

        // test
        public void SendUpdateFloorCommand(int floor, int room)
        {
            JObject json = new JObject
            {
                ["command"] = "UPDATE_FLOOR",
                ["value"] = $"{floor}"
            };

            SendJson(json);
        }
    }
}
