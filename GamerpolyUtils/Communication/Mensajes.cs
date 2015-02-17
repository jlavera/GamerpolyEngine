using GamerpolyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GamerpolyUtils.Communication {
    [Serializable]
    public class InicialServer : Communication<InicialServer>{
        public bool PideNuevaPartida;
        public int IdPartida;
        public int IdJugador;
        public InicialServer() { }
        public InicialServer(int? _id) : base(_id) { }
    }
    [Serializable]
    public class InicialServerResp : Communication<InicialServerResp> {
        public bool Otorga;
        public int Puerto;
        public int IdPartida;
        public InicialServerResp() { }
        public InicialServerResp(int? _id) : base(_id) { }
    }
    [Serializable]
    public class InicialPartida : Communication<InicialPartida> {
        public bool Arrancar;
        public InicialPartida() { }
        public InicialPartida(int? _id) : base(_id) { }
    }
    [Serializable]
    public class InicialPartidaResp : Communication<InicialPartidaResp> {
        public bool Conectado;
        public InicialPartidaResp() { }
        public InicialPartidaResp(int? _id) : base(_id) { }
    }
}
