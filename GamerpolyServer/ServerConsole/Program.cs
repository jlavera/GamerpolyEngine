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

            //https://www.google.com.ar/search?q=.net+multiplexing&oq=.net+multiplexing&aqs=chrome..69i57j0j69i65l3j69i61.23188j0j7&sourceid=chrome&es_sm=122&ie=UTF-8
            //http://forum.devmaster.net/t/c-sockets-threads-multiplexing/19449
            //https://msdn.microsoft.com/en-us/library/5w7b7x5f.aspx

            //--Cerrar los puertos y conexiones cuando ya no se usan

            Console.WriteLine("Starting server on port: {0} {1}", 3008, Environment.NewLine);

            Server srv = new Server().Start();

            srv.AceptarClientes(1);
            Console.Write("Input: ");
            string msj = Console.ReadLine();

            Console.WriteLine("{0}Broadcasting: " + msj, Environment.NewLine);
        }
    }
}