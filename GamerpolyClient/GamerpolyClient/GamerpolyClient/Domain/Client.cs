using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

using GamerpolyUtils;
using GamerpolyUtils.Communication;

namespace GamerpolyClient.Domain {
    public class Client {

        private TcpClient server;
        public int Id;

        public Client(int _id) {
            server = new TcpClient("127.0.0.1", 3008);
            Id = _id;
        }

        public int Create() {
            //--Manda el pedido de crear partida
            server.Snd(new InicialServer(Id) { PideNuevaPartida = true, IdJugador = Id });

            var obj = server.Rcv<InicialServerResp>();
            Console.WriteLine("Crear: {0} En: {1}", obj.Otorga, obj.Puerto);

            return obj.IdPartida;
        }

        public void Join(int id) {
            //--Manda el pedido de unirse a una partida
            server.Snd(new InicialServer(Id) { PideNuevaPartida = false, IdPartida = id, IdJugador = Id });

            var obj = server.Rcv<InicialServerResp>();
            Console.WriteLine("Join to: {0}", obj.Puerto);

            //--Cierra la conexión con el servidor
            server.Close();

            //--Se conecta a la partida
            server = new TcpClient("127.0.0.1", obj.Puerto);
            
            server.Snd(new InicialPartida(Id));

            var obj2 = server.Rcv<InicialPartidaResp>();
            Console.WriteLine("Conected {0}", obj2.Conectado);
        }

        public void Arrancar() {
            //--El creador manda la señal para arrancar la partida
            server.Snd(new InicialPartida() { Arrancar = true });
            Console.WriteLine("Arrancar partida");
        }

    }
}