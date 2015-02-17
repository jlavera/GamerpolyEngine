using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using GamerpolyUtils;
using GamerpolyUtils.Communication;

namespace GamerpolyServer.Domain {
    public class GameSystem {

        private TcpListener Listener;
        private List<Jugador> jugadores;
        private Jugador creador;
        private Thread lobby;
        private bool iniciar;

        public int Port { get { return ((IPEndPoint)Listener.LocalEndpoint).Port; } }

        public GameSystem(Jugador _creador, TcpListener _listener) {
            Listener = _listener;
            jugadores = new List<Jugador>() { _creador};
            creador = _creador;

            //--Iniciar el thread del juego, esperando la señal para comenzar
            new Thread(new ThreadStart(GameLoop)).Start();

            //--Iniciar el thread del lobby para ir agregando los jugadores
            lobby = new Thread(new ThreadStart(LobbyLoop));
            lobby.Start();
        }

        public void GameLoop() {
            iniciar = false;

            Console.WriteLine("Thread: Game Loop");
            //--Mientras que no esté cerrado el lobby, esperar señal de arranque
            while (!iniciar) {
                var obj = creador.Client.Rcv<InicialPartida>();
                iniciar = obj.Arrancar;
            }

            Console.WriteLine("Thread: Start game");

            ////--Fase de juego
            //while (true) {
            //    var client = jugadores[0];
            //    //client.Snd(tu turno);
            //    //client.Rcv(jugada);
            //}
            
            Console.WriteLine("Thread: End game");
        }

        public void LobbyLoop() {
            Console.WriteLine("Thread: Open Lobby");

            //--Fase de armado de partida, se unen los jugadores
            while (!iniciar) {
                var client = Listener.AcceptTcpClient();
                var obj = client.Rcv<InicialPartida>();

                Console.WriteLine("Thread: Agregando jugador {0}", obj.Id);
                jugadores.Add(new Jugador(obj.Id, client));
                client.Snd(new InicialPartidaResp() { Conectado = true });
            }

            Console.WriteLine("Thread: Close Lobby");
        }

        public void Multiplexar<T>() {
            Listener.BeginAcceptTcpClient(kcyo<T>, Listener);
        }

        public void kcyo<T>(IAsyncResult ar) {

        }
    }
}
