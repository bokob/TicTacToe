using System.Threading;

namespace TicTacToeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkServer server = new NetworkServer();
            server.Start();

            while (true)
            {
                server.PollEvents();
                Thread.Sleep(15); // 15ms 대기
            }
        }
    }
}