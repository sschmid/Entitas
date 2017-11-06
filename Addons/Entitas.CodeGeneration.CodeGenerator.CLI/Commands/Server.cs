using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Fabl;
using Fabl.Appenders;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Server : AbstractCommand {

        public override string trigger { get { return "server"; } }
        public override string description { get { return "Starts the Entitas server"; } }
        public override string example { get { return "entitas server port"; } }

        AbstractTcpSocket _socket;

        public override void Run(string[] args) {
            var port = 0;
            try {
                port = int.Parse(args[1]);
            } catch (Exception) {
                port = 3333;
            }

            var server = new TcpServerSocket();
            _socket = server;
            server.OnDisconnect += onDisconnect;
            server.Listen(port);
            _socket.OnReceive += onReceive;
            Console.CancelKeyPress += onCancel;
            while (true) {
                _socket.Send(Encoding.UTF8.GetBytes(Console.ReadLine()));
            }
        }

        void onReceive(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            var message = Encoding.UTF8.GetString(bytes);
            fabl.Info(message);

            var args = getArgsFromMessage(message);

            try {
                Program.GetCommand(args[0]).Run(args);
            } catch (Exception ex) {
                Program.PrintException(ex, args);
            }
        }

        string[] getArgsFromMessage(string command) {
            return command
                .Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }

        void onCancel(object sender, ConsoleCancelEventArgs e) {
            _socket.Disconnect();
        }

        void onDisconnect(AbstractTcpSocket socket) {
            Environment.Exit(0);
        }
    }
}
