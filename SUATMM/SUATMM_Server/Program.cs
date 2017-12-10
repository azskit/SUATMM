using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SUATMM
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "localhost";
            string port = "13000";

            foreach (string arg in args)
            {
                if (arg.ToLowerInvariant().StartsWith("site="))
                    host = arg.Split('=')[1];
                else if (arg.ToLowerInvariant().StartsWith("port="))
                    port = arg.Split('=')[1];
                else
                {
                    Console.WriteLine("usage: SuatmmServer.exe [site=<site>] [port=<port>]");
                    Console.WriteLine("example: SuatmmServer.exe site=thisserver.* port=8080");
                    return;
                }
            }

            using (SuatmmServer server = new SuatmmServer(host, port))
            {

                server.Out = Console.Out;

                string command = "start";
                while (command != "exit")
                {
                    switch (command)
                    {
                        case "start":
                            server.Run();
                            break;

                        case "stop":
                            server.Stop();
                            break;

                        default:
                            Console.WriteLine("Available commands: start stop exit");
                            break;
                    }

                    command = Console.ReadLine().ToLower();
                }
            }

        }
    }
}
