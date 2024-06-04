using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace network;

class Program
{
    static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
    private static CancellationToken token = cancelTokenSource.Token;

    public static async Task Main(string[] args)
    {
        await Task.Run(() => {Server();},token);
    }
    
    public static void Server()
    {
        UdpClient udpClient = new UdpClient(12345);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Console.WriteLine("Сервер ждёт сообщения от клиента");
        

        
        while (true)
        {
            byte[] buffer = udpClient.Receive(ref ipEndPoint);
            var messageText = Encoding.UTF8.GetString(buffer);
            Message? message = Message.DeserializeFromJson(messageText);
            message?.Print();
            if (message?.Text?.ToLower() == "exit")
            {
                cancelTokenSource.Cancel();
            }

            if (token.IsCancellationRequested)
            {
                cancelTokenSource.Dispose();
                return;
            }
            Task task = Task.Run(SendReply);

            void SendReply()
            {
                // отправка ответа от сервера  
                Console.WriteLine("Отправлено ответное сообщение клиенту");
                byte[] data = "Сообщение получено"u8.ToArray();
                udpClient.Send(data, data.Length, ipEndPoint);
                
            }
        }
    }
}