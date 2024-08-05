using EleCho.GoCqHttpSdk;
using System.Net.Sockets;
using System.Net;
using System.Text;

 static string SocketServer(string ServerIP,int Port,string Text)
{
    try
    {
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect(IPAddress.Parse(ServerIP), Port);
    byte[] bytes = Encoding.UTF8.GetBytes(Text);
    socket.Send(bytes);
    byte[] array = new byte[1024];
    int count = socket.Receive(array);
    string text = Encoding.UTF8.GetString(array, 0, count);
    return text;
    }
    catch (Exception)
    {

        return "null";
    }
}


Console.WriteLine("\r\nWelcome DIRSYSTEM-V1.2.0 Byyudir");
Console.WriteLine("\r\n[DIRSystem]: 使用前请确保打开了正向WebSocket6700端口 服务器端打开TCP端口");
Console.WriteLine();
Console.WriteLine("请输入服务器查询端口(如有多个请用'*'隔开 如:10087*10089*10092):");
string ports = Console.ReadLine();
string[] port1 = new string[] { ports };
if (ports.Contains("*"))
{
    port1 = ports.Split('*');
}
Console.WriteLine("请输入对应的QQ群号(如有多个请用'*'隔开 如:10087*10089*10092):");
string qq = Console.ReadLine();
string[] strings = new string[] { qq};
if (qq.Contains("*"))
{

  strings = qq.Split('*');
}
Console.WriteLine("\r\n[DIRSystem]: 使用Q群指令:cx,info,round(功能仅限本群管理员与管理员能用)");
Console.WriteLine("\r\n[DIRSystem]: 正在连接WebSK-Success-Sk6700端口");
CqWsSession session = new CqWsSession(new CqWsSessionOptions
{
    BaseUri = new Uri("ws://localhost:6700"),  // WebSocket 地址
});
Console.WriteLine("\r\n[DIRSystem]: Binggo 连接成功 请将我挂在后台哦");
session.UseGroupMessage(async context =>
{
    if (strings.Contains(context.GroupId.ToString()))
    {
        if (context.Message.Text.Contains("cx"))
        {
            string total = string.Empty;

            foreach (var item in port1)
            {
                string wda = SocketServer("127.0.0.1", int.Parse(item), "cx");
                if (wda == "null")
                {
                    total += $"#{port1}服不在线 awa";
                }
                else
                {
                    total += wda;
                }

            }
            await session.SendGroupMessageAsync(context.GroupId, new EleCho.GoCqHttpSdk.Message.CqMessage(total));
            //Console.WriteLine($"收到来自{context.GroupId}的{context.UserId}的查询消息返回{a}");
        }
        else if (context.Message.Text.Contains("info"))
        {
            string total = "";
            foreach (var item1 in port1)
            {
                string wda = SocketServer("127.0.0.1", int.Parse(item1), "info");
                if (wda == "null")
                {
                    total += $"#{item1}服不在线 awa";
                }
                else
                {
                    total += wda;
                }
            }
            await session.SendGroupMessageAsync(context.GroupId, new EleCho.GoCqHttpSdk.Message.CqMessage(total));
        }
    };
    await session.RunAsync();
});

