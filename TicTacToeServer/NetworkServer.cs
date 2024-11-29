using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TicTacToeServer
{
    public class NetworkServer : INetEventListener
    {
        NetManager _netManager;                 // 네트워크 작업을 관리하는 주요 클래스, 클라이언트 및 서버로 사용될 수 있음
        Dictionary<int, NetPeer> _connections;  // NetPeer: 특정 클라이언트와 서버 간의 연결을 나타내는 객체, IP주소, 포트 등을 갖고 있음

        // 서버 시작
        public void Start()
        {
            _connections = new Dictionary<int, NetPeer>();

            _netManager = new NetManager(this)
            {
                DisconnectTimeout = 100000 // 해당 시간 동안 패킷을 받지 않으면 연결 종료하는 시간
            };

            _netManager.Start(9050);

            Console.WriteLine("Server listening on port 9050");
        }

        // 보류 중인 모든 이벤트를 수신, 게임 업데이트에서 호출해야 함
        public void PollEvents()
        {
            _netManager.PollEvents();
        }

        // 연결 요청이 들어왔을 때 호출되는 콜백 함수
        public void OnConnectionRequest(ConnectionRequest request)
        {
            Console.WriteLine($"Incomming connection from {request.RemoteEndPoint}");
            request.Accept();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            // throw new NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            //throw new NotImplementedException();
        }

        // 데이터를 수신했을 때 호출되는 콜백 함수
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            var data = Encoding.UTF8.GetString(reader.RawData);
            Console.WriteLine($"Data received from client: '{data}'");

            // 클라이언트에게 응답
            var reply = "Reply to client";
            var bytes = Encoding.UTF8.GetBytes(reply);
            peer.Send(bytes, DeliveryMethod.ReliableOrdered);
        }
        
        // 새로운 Peer가 연결될 때 호출되는 콜백 함수
        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine($"Client connected to server: {peer.EndPoint}, Id: {peer.Id}");
            _connections.Add(peer.Id, peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine($"{peer.EndPoint} disconnected");
            _connections.Remove(peer.Id);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            //throw new NotImplementedException();
        }
    }
}