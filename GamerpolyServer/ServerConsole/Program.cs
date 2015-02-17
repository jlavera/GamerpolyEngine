using GamerpolyServer;
using GamerpolyServer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerConsole {
    class Program {
        static void Main(string[] args) {

            //TODO
            //--Multiplexor o la tele. La tele la tele!!
            //--Cerrar los puertos y conexiones cuando ya no se usan

            Console.WriteLine("Starting server on port: {0} {1}", 3008, Environment.NewLine);

            Server srv = new Server().Start();

            srv.AceptarClientes(1);
            Console.Write("Input: ");
            string msj = Console.ReadLine();

            Console.WriteLine("{0}Broadcasting: " + msj, Environment.NewLine);

            //srv.Broadcast(msj);
        }
    }
}