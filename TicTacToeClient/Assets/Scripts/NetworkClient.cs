using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkClient : MonoBehaviour, INetEventListener
{
    static NetworkClient _instance;

    public static NetworkClient Instance { get { return _instance; } }

    public event Action OnServerConnected;

    NetManager _netManager;
    NetPeer _server;        // NetPeer: 특정 peer에게 메시지를 보내는 역할 (공식문서), 여기서는 서버를 의미함
    NetDataWriter _writer;

    void Awake()
    {
        if(_instance != null)
        {
            Destroy(_instance);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        Init();
    }


    void Update()
    {
        _netManager.PollEvents();
    }

    public void Init()
    {
        _writer = new NetDataWriter();
        _netManager = new NetManager(this)
        {
            DisconnectTimeout = 100000
        };
        _netManager.Start();
    }

    public void Connect() // 서버에 연결
    {
        _netManager.Connect("localhost", 9050, "");
    }

    // 서버로 송신
    public void SendServer<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable
    {
        if(_server == null)
            return;

        _writer.Reset();
        packet.Serialize(_writer);
        _server.Send(_writer, deliveryMethod);
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) // 서버로부터 수신
    {
        var data = Encoding.UTF8.GetString(reader.RawData).Replace("\0", "");
        Debug.Log($"Data received from server: {data}");
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        throw new System.NotImplementedException();
    }

    // 클라이언트 또는 서버가 다른 NetPeer와 성공적으로 연결되었을 때 호출되는 콜백 메서드, 여기서는 서버에 연결될 때 호출됨
    public void OnPeerConnected(NetPeer peer) 
    {
        Debug.Log("We connected to server at " + peer.EndPoint);
        _server = peer;
        OnServerConnected?.Invoke();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("Lost connection to server!");
    }
}
