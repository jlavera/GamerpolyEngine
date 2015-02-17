using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GamerpolyUtils {
    [Serializable]
    public abstract class Communication<T> {

        public int Id{get;set;}

        public Communication() { }
        public Communication(int? _id) {
            if (_id.HasValue)
                Id = _id.Value;
        }

        public byte[] Serialize() {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public T Deserialize(byte[] buffer) {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(buffer, 0, buffer.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (T)binForm.Deserialize(memStream);
        }
    }
}