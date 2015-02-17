using GamerpolyClient.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole {
    class Program {
        static void Main(string[] args) {

            Console.WriteLine();

            Client cli = new Client(1);
            int id = cli.Create();

            Console.ReadKey();

            Client cli2 = new Client(2);
            cli2.Join(id);

            Console.ReadKey();

            cli.Arrancar();

            Console.ReadKey();
        }
    }
}
