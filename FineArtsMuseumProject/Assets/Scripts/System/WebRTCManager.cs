using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using DesignPatterns;
using Network;
using Player;
using TMPro;
using Unity.RenderStreaming;
using Unity.WebRTC;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class WebRTCManager : MonoSingleton<WebRTCManager>
{
    [field: SerializeField] private SignalingManager signalingManager;
    
    //[field: SerializeField] private TMP_Text ipText;
    [field: SerializeField] private VideoStreamSender videoStreamSender;
    [field: SerializeField] private AudioStreamSender audioStreamSender;
    [field: SerializeField] private InputReceiver inputReceiver;
    [field: SerializeField] private VideoStreamReceiver videoStreamReceiver;
    [field: SerializeField] private AudioStreamReceiver audioStreamReceiver;
    [field: SerializeField] private Broadcast broadcast;
    [field: SerializeField] private SocketManager socketManager;
    
    [Header("Debug")]
    [field: SerializeField] private bool debugMode;
    
    private string _listenPort = "";
    
    private void Awake()
    {
        if(SceneLog.IsFirstScene)
            DontDestroyOnLoad(this.gameObject);
        else
        {
            if(PlatformManager.Instance.isSplitScene) return;
            Destroy(this.gameObject);
        }
    }
    
    void Start()
    {
        if(!PlatformManager.Instance.IsWebGL && !PlatformManager.Instance.IsCloud) return;
        if (debugMode)
        {
            TestScript();
        }
        else
        {
            StartCoroutine(ListenPort());
        }
    }

    private void TestScript()
    {
        const int port = 8000;

        var signalingURL = $"ws://127.0.0.1:{port}";

        signalingManager.Stop();

        var setting = new WebSocketSignalingSettings(
            url: signalingURL,
            new []
            {
                new IceServer(urls: new[] {"stun:stun.l.google.com:19302"}),
                new IceServer(urls: new[] { "turn:shareteam3.ddns.net:3478?transport=udp" },
                    username: "phong",
                    credential: "123456",
                    credentialType: IceCredentialType.Password)
            }
        );

        signalingManager.SetSignalingSettings(setting);
        
        videoStreamSender.source = VideoStreamSource.Screen;
        videoStreamSender.SetTextureSize(new Vector2Int(Screen.width, Screen.height));
        broadcast.AddComponent(videoStreamSender);
        audioStreamSender.source = AudioStreamSource.AudioListener;
        broadcast.AddComponent(audioStreamSender);
        broadcast.AddComponent(inputReceiver);
        broadcast.AddComponent(videoStreamReceiver);
        broadcast.AddComponent(audioStreamReceiver);
        signalingManager.AddSignalingHandler(broadcast);
        signalingManager.Run();
    }

    private IEnumerator ListenPort()
    {
        while (_listenPort == "")
        {
            string[] args = Environment.GetCommandLineArgs();

            foreach (string arg in args)
            {
                if (!arg.StartsWith("--webrtc-port=")) continue;
                try
                {
                   //var webrtcPort = int.Parse(arg.Split('=')[1]);
                   //_listenPort = webrtcPort;
                   var value = arg.Split('=')[1];
                   var fields = value.Split('|');
                   _listenPort = fields[0];
                   var signalingURL = $"{_listenPort}";

                    signalingManager.Stop();

                    var setting = new WebSocketSignalingSettings(
                        url: signalingURL,
                        new []
                        {
                            new IceServer(urls: new[] {"stun:stun.l.google.com:19302"}),
                            new IceServer(urls: new[] { "turn:turn-server-1.daiquocviet.vn:3478?transport=udp" },
                                username: "turn-server-1",
                                credential: "123456",
                                credentialType: IceCredentialType.Password),
                            new IceServer(urls: new[] { "turn:shareteam3.ddns.net:3478?transport=udp" },
                                username: "phong",
                                credential: "123456",
                                credentialType: IceCredentialType.Password)
                        }
                    );

                    signalingManager.SetSignalingSettings(setting);
                    
                    videoStreamSender.source = VideoStreamSource.Screen;
                    videoStreamSender.SetTextureSize(new Vector2Int(Screen.width, Screen.height));
                    broadcast.AddComponent(videoStreamSender);
                    
                    audioStreamSender.source = AudioStreamSource.AudioListener;
                    broadcast.AddComponent(audioStreamSender);
                    broadcast.AddComponent(inputReceiver);
                    broadcast.AddComponent(videoStreamReceiver);
                    broadcast.AddComponent(audioStreamReceiver);
                    signalingManager.AddSignalingHandler(broadcast);
                    signalingManager.Run();

                    //ipText.text += "Đã chạy" + " | ";

                    if (fields.Length > 1)
                    {
                        var socketString = fields[1];
                        var socketFields = socketString.Split(':');
                        if(socketFields.Length != 2) continue;
                        var ip = socketFields[0];
                        var port = int.Parse(socketFields[1]);
                        socketManager.InitSocket(ip, port);
                    }
                    
                    break;
                }
                catch (Exception e)
                {
                    //ipText.text += " | " + e.Message;
                }
            }
            yield return new WaitForSeconds(1f);
        }
        
        if(_listenPort != "") yield break;

        yield return new WaitForSeconds(1f);
    }

    public void RegisterMoveInput(System.Action<InputAction.CallbackContext> onMovePerformed)
    {
        if (!inputReceiver) return;
        var moveAction = inputReceiver.actions;
        if (!moveAction) return;
        moveAction["Move"].performed += onMovePerformed;
        moveAction["Move"].canceled += onMovePerformed;
    }
    
    public void RegisterRotateCameraInput(System.Action<InputAction.CallbackContext> onRotatePerformed)
    {
        if (!inputReceiver) return;
        var rotateAction = inputReceiver.actions;
        if (!rotateAction) return;
        rotateAction["Move"].performed += onRotatePerformed;
        rotateAction["Move"].canceled += onRotatePerformed;
    }

    public void RegisterAudioListener(AudioSource audioListener)
    {
        if(!audioStreamReceiver) return;
        audioStreamReceiver.targetAudioSource = audioListener;
    }
}