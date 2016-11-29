using System;
using NetMQ;
using NetMQ.Sockets;

namespace CardMessageSubscriber
{
    internal class Program
    {
        private static readonly string pubAddress = "tcp://127.0.0.1:5883";

        private static void Main(string[] args)
        {
            string _channel = "CardUID";
            using (var subSocket = new SubscriberSocket(pubAddress))
            {
                using (var poller = new NetMQPoller { subSocket })
                {
                    Console.WriteLine("Press CTRL+C to stop ...");
                    subSocket.Subscribe(_channel);
                    subSocket.ReceiveReady += (sender, e) =>
                    {
                        var channel = e.Socket.ReceiveFrameString();
                        if (channel.Equals(_channel))
                        {
                            var uid = e.Socket.ReceiveFrameString();
                            Console.WriteLine("Received Card UID = {0}", uid);
                        }
                    };
                    poller.Run();
                }
            }
        }
    }
}