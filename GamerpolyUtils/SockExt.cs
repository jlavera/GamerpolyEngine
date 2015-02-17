using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GamerpolyUtils {
    static public class SockExt {

        //--TODO falta encriptar el mensaje hehehhe


        static public T Rcv<T>(this TcpClient cli) where T : Communication<T>, new() {
            var buffer = new byte[256];
            cli.GetStream().Read(buffer, 0, buffer.Length);
            return new T().Deserialize(buffer);
        }

        static public void Snd<T>(this TcpClient cli, Communication<T> obj) {
            byte[] msg = obj.Serialize();
            cli.GetStream().Write(msg, 0, msg.Length);
        }

    }
}