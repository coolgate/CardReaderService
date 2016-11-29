using CardTools;
using NLog;
using System;
using System.Runtime.Remoting.Channels;
using NetMQ;
using NetMQ.Sockets;
using Topshelf;
using WebSocketSharp;
using WebSocketSharp.Server;
using Logger = NLog.Logger;

namespace ReadCardService
{
    class CardReaderWebSocketService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {            
        }
    }
    class ReaderService
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        private CardReader _reader;
        private string pubAddress = "tcp://127.0.0.1:5883";
        private string _channel = "CardUID";
        private PublisherSocket _pubsock;
        private string socketServerAddress = "ws://127.0.0.1:5884";
        private WebSocketServer _socketServer;

        public ReaderService()
        {
        }

        public bool Start()
        {
            _reader = new CardReader();
            _reader.HoldTime = 2000;
            _reader.ReadInterval = 1000;

            // Simple Logger
            _reader.OnReadCardUid += (sender, e) =>
            {
                var msg = string.Format("Read Card, UID = {0}", e.Uid);
                Console.WriteLine(msg);
                _log.Info(msg);
            };

            // NetMQ Serive
            _pubsock = new PublisherSocket(pubAddress);
            _reader.OnReadCardUid += (sender, e) =>
            {
                var ok = _pubsock.SendMoreFrame(_channel).TrySendFrame(e.Uid);
                if (ok)
                {
                    var msg = string.Format("Publish Message: Card UID = {0}", e.Uid);
                    _log.Debug(msg);
                }
            };

            // WebSocket Service
            _socketServer = new WebSocketServer(socketServerAddress);
            _socketServer.AddWebSocketService<CardReaderWebSocketService>("/CardReader");
            _reader.OnReadCardUid += (sender, e) =>
            {
                if (_socketServer.IsListening)
                {
                    var msg = string.Format("{{\"uid\": \"{0}\"}}", e.Uid);
                    _socketServer.WebSocketServices["/CardReader"].Sessions.Broadcast(msg);
                }                
            };
            _socketServer.Start();

            Console.WriteLine("Press CTRL+C to stop ...");

            _reader.OpenDevice();
            _reader.ReadForever();

            return true;
        }

        public bool Stop()
        {
            _reader.StopReadForever();
            _reader.CloseDevice();
            _pubsock.Close();
            _socketServer.Stop();

            _log.Info("CardReaderService Stopped");

            return true;
        }


        public bool Pause(HostControl hostControl)
        {
            _log.Info("CardReaderService Paused");

            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            _log.Info("CardReaderService Continued");

            return true;
        }

    }

    class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.UseNLog();

                x.Service<ReaderService>(s =>
                {
                    s.ConstructUsing(() => new ReaderService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
//                x.EnableServiceRecovery(r =>
//                {
//                    r.RestartService(3);
//                });

//                x.RunAsLocalSystem();

                x.SetDescription("Card Reader Service");
                x.SetDisplayName("CardReaderService");
                x.SetServiceName("CardReaderService");
            });

        }
    }
}