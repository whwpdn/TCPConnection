using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace TCPConnection
{
    public enum PacketType : int
    {
        Login =0,
        Login_Result,
        RegisterUser,
        RegisterUser_Result
    }
    [Serializable]
    class Packet
    {
        public int packetType;
        public int packetLength;

        public Packet()
        {
            this.packetType = 0;
            this.packetLength = 0;
        }

        public static byte[] Serialize(Object data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(1024 * 4);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static Object Deserialize(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(1024 *4);
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                BinaryFormatter bf = new BinaryFormatter();
                Object obj = bf.Deserialize(ms);
                ms.Close();
                return obj;

            }
            catch
            {
                return null;
            }
        }
    }
}
