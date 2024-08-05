using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Log = Exiled.API.Features.Log;
using Player = Exiled.API.Features.Player;

namespace SocketServer
{
    public class Config : IConfig
    { 
        public int TcpPort { get; set; } = 10087;
        public string IP { get; set; } = "127.0.0.1";
        [Description("服务器最高人数上限")]
        public string MaxPlayer { get; set; } = "40";
        public bool IsEnabled { get ; set ; }= true;
        public bool Debug { get ; set ; } = false;
    }
    public class Main : Plugin<Config>
    {
        public override string Author => "yudir";
        public override string Name => "CX";
        public override Version Version => new Version(1, 2, 0);
        public static Main Maina;

        public override void OnEnabled()
        {
            Maina = this;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Wait;
            base.OnEnabled();
        }

        public void Wait()
        {
            Task.Run(delegate ()
            {
                int port2 = Maina.Config.TcpPort;
                IPAddress any = IPAddress.Parse(Maina.Config.IP);
                var ReceiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipendPoint = new IPEndPoint(any, port2);
                ReceiveSocket.Bind(new IPEndPoint(any, port2));
                ReceiveSocket.Listen(10);
                while (true)
                {

                    Socket socket = ReceiveSocket.Accept();
                    byte[] array = new byte[1024];
                    int count = socket.Receive(array);
                    string returna = Encoding.UTF8.GetString(array, 0, count);
                    if (returna == "cx" || returna == "查询" || returna == "cxk")
                    {
                        string a = string.Empty;
                        a += $"東山怀旧服 - CX查询系统\r\n";
                        a += $"\r\n当前人数：{Player.List.Count().ToString()}/{Maina.Config.MaxPlayer}\r\n";
                        if (Round.IsStarted)
                        {
                            a += $"回合信息：正在进行\r\n";
                        }
                        else if (Round.IsLobbyLocked)
                        {
                            a += $"回合信息：暂停开始回合\r\n";
                        }
                        else if (Round.IsLocked)
                        {
                            a += $"回合信息：回合暂停\r\n";
                        }
                        else
                        {
                            a += $"回合信息：等待玩家中\r\n";
                        }
                        a += "\r\n查询时间 " + DateTime.Now;
                        a += $"\r\n光想着查询怎能行？进服才是真理！";
                        byte[] bytes = Encoding.UTF8.GetBytes(a);
                        socket.Send(bytes);
                        socket.Close();
                    }

                    else if (returna == "info" || returna == "信息" || returna == "对局" || returna == "回合" || returna == "DD")
                    {
                        string a = string.Empty;
                        a += $"東山怀旧服 - CX查询系统\r\n";

                        a += $"\r\nDD人数: {Player.Get(PlayerRoles.RoleTypeId.ClassD).Count()}";
                        a += $"\r\n博士人数: {Player.Get(PlayerRoles.RoleTypeId.Scientist).Count()}";
                        a += $"\r\nSCP人数: {Player.Get(PlayerRoles.Team.SCPs).Count()}";
                        a += $"\r\n回合已进行：{Round.ElapsedTime.Minutes}\r\n";

                        a += "\r\n查询时间" + DateTime.Now;
                        byte[] bytes = Encoding.UTF8.GetBytes(a);
                        socket.Send(bytes);
                        socket.Close();

                    }
                    else if (returna == ("list"))
                    {
                        string aaa = string.Empty;
                        aaa += $"東山怀旧服 - CX查询系统\r\n";
                        foreach (var item in Player.List)
                        {
                            aaa += $"\r\n{item.Nickname}";
                        }
                        aaa += "\r\n查询时间" + DateTime.Now;
                        byte[] bytes = Encoding.UTF8.GetBytes(aaa);
                        socket.Send(bytes);
                        socket.Close();
                    }
                }
            });
            Log.Debug("Started");
        }
    }
}
