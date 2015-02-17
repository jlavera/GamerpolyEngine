using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GamerpolyServer.Utils {
    public class Utils {

        /// <summary>
        /// Retorna un listener en un puerto libre.
        /// </summary>
        /// <returns></returns>
        static public TcpListener GetFreeTcp() {
            TcpListener l = new TcpListener(IPAddress.Parse("127.0.0.1"), 0);
            l.Start();
            return l;
        }

    }
}