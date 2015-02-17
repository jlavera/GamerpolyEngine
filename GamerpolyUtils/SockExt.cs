using GamerpolyUtils.Security;
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

        static public T Rcv<T>(this TcpClient cli) where T : Communication<T>, new() {
            var buffer = new byte[256];
            cli.GetStream().Read(buffer, 0, buffer.Length);
            return new T().Deserialize(SimpleAES.Decrypt(buffer));
        }

        static public void Snd<T>(this TcpClient cli, Communication<T> obj) {
            byte[] msg = SimpleAES.Encrypt(obj.Serialize());
            cli.GetStream().Write(msg, 0, msg.Length);
        }

    }
}