using LiteNetLib.Utils;

namespace NetworkShared
{
    // 주고 받을 패킷의 타입 정의
    public enum PacketType : byte
    {
        // 클라이언트 -> 서버
        #region ClientServer
        Invalid = 0,
        AuthRequest = 1,
        #endregion

        // 서버 -> 클라이언트
        #region ServerClient
        OnAuth = 100
        #endregion
    }

    public interface INetPacket : INetSerializable
    {
        PacketType Type { get; }
    }
}
