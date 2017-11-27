using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Net;


namespace TcpServerForm
{
    class HandleClient
    {
        TcpClient clientSocket;
        int clientNo;
        Thread thHandler;
        public string userid {get; set;}
        public void StartClient(TcpClient ClientSocket, int clientNo)
        {
            this.clientSocket = ClientSocket;
            this.clientNo = clientNo;
            //Thread thHandler = new Thread(DoChatting);
            thHandler = new Thread(GetPacket);
            thHandler.IsBackground = true;
            thHandler.Start();
            
        }

        //public delegate void MessageDisplayHandler(string text);
        //public event MessageDisplayHandler OnReceived;

        public delegate void CalculateClientCounter(string address, int no);
        public event CalculateClientCounter OnCalculated;

        public delegate void AnalyzePacket(NetworkStream stream, byte[] buffer);
        public event AnalyzePacket OnPacketReceived;

        private void GetPacket()
        {
            NetworkStream stream = null;
            try
            {
                byte[] buff = new byte[1024*4];
                //string msg = string.Empty;
                string tempPacket = string.Empty;
                int iBytes = 0;
                //int iMessageCount = 0;
                //int iLastIndex = 0;
                while (true)
                {
                    stream = this.clientSocket.GetStream();
                    iBytes = stream.Read(buff, 0, buff.Length);
                    //msg = Encoding.Unicode.GetString(buff, 0, iBytes);
                    
                    //tempPacket = msg.Substring(iLastIndex, msg.IndexOf("$", iLastIndex));
                    //iLastIndex = msg.IndexOf("$");
                    //if(iLastIndex>0)
                    //    msg = msg.Substring(0, msg.IndexOf("$"));
                    //msg= "received Data : " + msg;

                    if (OnPacketReceived != null)
                        OnPacketReceived(stream, buff);
                    if (iBytes == 0)
                    {
                        OnCalculated(((IPEndPoint)this.clientSocket.Client.RemoteEndPoint).Address.ToString(), clientNo);
                        if (clientSocket.Connected)
                        {
                            this.clientSocket.Client.Disconnect(true);
                            this.clientSocket.Close();
                        }
                        
                        stream.Close();
                        break;
                    }                  
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(string.Format("GetPackSocketException : {0}", se.Message));

                if (this.clientSocket != null)
                {
                    this.clientSocket.Close();
                    stream.Close();
                }

                //if (OnCalculated != null)
                 //   OnCalculated(((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("GetPackException : {0}", ex.Message));
                if (this.clientSocket != null)
                {
                    this.clientSocket.Close();
                    stream.Close();
                }

                //if (OnCalculated != null)
                 //   OnCalculated(((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString());
            }
            finally
            {
                if (this.clientSocket != null)
                {
                    if (this.clientSocket.Connected)
                    {
                        this.clientSocket.Close();
                        

                    }
                    stream.Close();
                }
                thHandler.Abort();
            }
        }
    }
}
