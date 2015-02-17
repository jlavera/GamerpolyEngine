using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GamerpolyServer.Domain {
    public class Jugador {
        public int JugadorId { get; set; }
        public TcpClient Client { get; set; }

        public Jugador(int _id, TcpClient _cli) {
            JugadorId = _id;
            Client = _cli;
        }
    }
}
