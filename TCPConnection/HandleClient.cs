using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace TCPConnection
{
    class HandleClient
    {
        TcpClient clientSocket;
        int clientNo;

        public void StartClient(TcpClient ClientSocket, int clientNo)
        {
            this.clientSocket = ClientSocket;
            this.clientNo = clientNo;
            //Thread thHandler = new Thread(DoChatting);
            Thread thHandler = new Thread(GetPacket);
            thHandler.IsBackground = true;
            thHandler.Start();
            
        }

        public delegate void MessageDisplayHandler(string text);
        public event MessageDisplayHandler OnReceived;

        public delegate void CalculateClientCounter();
        public event CalculateClientCounter OnCalculated;

        private void GetPacket()
        {
            NetworkStream stream = null;
            try
            {
                byte[] buff = new byte[1024];
                string msg = string.Empty;
                string tempPacket = string.Empty;
                int iBytes = 0;
                //int iMessageCount = 0;
                int iLastIndex = 0;
                while (true)
                {
                    //iMessageCount++;

                    stream = clientSocket.GetStream();
                    iBytes = stream.Read(buff, 0, buff.Length);
                    msg = Encoding.Unicode.GetString(buff, 0, iBytes);
                    
                    //tempPacket = msg.Substring(iLastIndex, msg.IndexOf("$", iLastIndex));
                    iLastIndex = msg.IndexOf("$");
                    
                    //msg = msg.Substring(0, msg.IndexOf("$"));
                    
                    //msg= "received Data : " + msg;

                    if (OnReceived != null)
                        OnReceived(tempPacket);

                    //msg = "Server to client(" + clientNo.ToString() + ")"; // + iMessageCount.ToString();

                    //if (OnReceived != null)
                    //    OnReceived(msg);

                    //byte[] subBuff = Encoding.Unicode.GetBytes(msg);
                    //stream.Write(subBuff, 0, subBuff.Length);
                    //stream.Flush();

                    //msg = ">>" + msg;
                    //if (OnReceived != null)
                    //{
                    
                    //    OnReceived("");
                    //}
                }
            }
            catch (SocketException se)
            {
                Trace.WriteLine(string.Format("DoChatting - SocketException : {0}", se.Message));

                if (clientSocket != null)
                {
                    clientSocket.Close();
                    stream.Close();
                }

                if (OnCalculated != null)
                    OnCalculated();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("DoChatting - Exception : {0}", ex.Message));
                if (clientSocket != null)
                {
                    clientSocket.Close();
                    stream.Close();
                }

                if (OnCalculated != null)
                    OnCalculated();
            }
        
        }
        private void DoChatting()
        {
            NetworkStream stream = null;
            try
            {
                byte[] buff = new byte[1024];
                string msg = string.Empty;
                int iBytes = 0;
                int iMessageCount = 0;

                while (true)
                {
                    iMessageCount++;
                    stream = clientSocket.GetStream();
                    iBytes = stream.Read(buff, 0, buff.Length);
                    msg = Encoding.Unicode.GetString(buff, 0, iBytes);
                    msg = msg.Substring(0, msg.IndexOf("$"));
                    
                    msg = "received Data : " + msg;

                    if (OnReceived != null)
                        OnReceived(msg);

                    msg = "Server to client(" + clientNo.ToString() + ")" + iMessageCount.ToString();

                    if (OnReceived != null)
                        OnReceived(msg);

                    byte[] subBuff = Encoding.Unicode.GetBytes(msg);
                    stream.Write(subBuff, 0, subBuff.Length);
                    stream.Flush();

                    msg = ">>" + msg;
                    if (OnReceived != null)
                    {
                        OnReceived(msg);
                        OnReceived("");
                    }
                }
            }
            catch (SocketException se)
            {
                Trace.WriteLine(string.Format("DoChatting - SocketException : {0}", se.Message));

                if (clientSocket != null)
                {
                    clientSocket.Close();
                    stream.Close();
                }

                if (OnCalculated != null)
                    OnCalculated();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("DoChatting - Exception : {0}", ex.Message));
                if (clientSocket != null)
                {
                    clientSocket.Close();
                    stream.Close();
                }

                if (OnCalculated != null)
                    OnCalculated();
            }
        }
    }
}
