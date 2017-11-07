using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Fabl;
using Fabl.Appenders;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Server : AbstractCommand {

        public override string trigger { get { return "server"; } }
        public override string description { get { return "Start server mode (default port is 3333)"; } }
        public override string example { get { return "entitas server port"; } }

        AbstractTcpSocket _socket;
        List<string> _logBuffer = new List<string>();

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
                if (args[0] == trigger) {
                    throw new Exception("Server is already running!");
                }
                var command = Program.GetCommand(args[0]);
                fabl.AddAppender(onLog);
                command.Run(args);
                fabl.RemoveAppender(onLog);
                socket.Send(Encoding.UTF8.GetBytes(getLogBufferString()));
            } catch (Exception ex) {
                Program.PrintException(ex, args);
                socket.Send(Encoding.UTF8.GetBytes(getLogBufferString() + ex.Message));
            }

            _logBuffer.Clear();
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

        string getLogBufferString() {
            return string.Join("\n", _logBuffer.ToArray());
        }

        void onLog(Logger logger, LogLevel loglevel, string message) {
            _logBuffer.Add(message);
        }
    }
}
