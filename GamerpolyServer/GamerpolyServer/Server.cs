using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using GamerpolyUtils;
using GamerpolyServer.Domain;
using GamerpolyUtils.Communication;

namespace GamerpolyServer {
    public class Server {

        private TcpListener listener;
        private List<TcpClient> clients;
        private Dictionary<int, GameSystem> currentGames;
        private int currentId;
        private int currentPort; //--TODO ver como obtener un puerto libre dentro de un rango

        private bool escuchar;

        public Server() {
            clients = new List<TcpClient>();
            currentGames = new Dictionary<int, GameSystem>();
            currentId = 0;

            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3008);
        }

        public Server Start() {
            escuchar = true;
            listener.Start();

            return this;
        }

        public void AceptarClientes(int cant) {
            while (true) {
                var client = listener.AcceptTcpClient();

                //--Obtiene un listener en un puerto vacío
                var list = Utils.Utils.GetFreeTcp();
                
                //--Ver si quiere crear o unise
                var obj = client.Rcv<InicialServer>();

                if (obj.PideNuevaPartida) {
                    //--Agregar partida nueva con el creador
                    currentGames.Add(currentId, 
                        new GameSystem(
                            new Jugador(obj.IdJugador, client),
                            list));

                    client.Snd(
                        new InicialServerResp() {
                            IdPartida = currentId,
                            Otorga = true,
                            Puerto = ((IPEndPoint)list.LocalEndpoint).Port
                        });
                    Console.WriteLine("Create {0}", currentId);
                } else {
                    //--Buscar juego existente
                    var gs = currentGames[obj.IdPartida];
                    client.Snd(
                        new InicialServerResp() {
                            Puerto = gs.Port
                        });
                    Console.WriteLine("Join a {0}", gs.Port);
                }

                currentId++;
            }
        }
    }
}