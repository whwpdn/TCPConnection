using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using MyPackets;


namespace TcpClientForm
{

    public partial class ClientForm : Form
    {
        class TimeLeft
        {
            static public int hour;
            static public int min;
            static public int sec;

            static public int allTimeValue;
            //static public int nowTimeValue;
        }

        TcpClient clientSocket;
        const string serverAddr = "127.0.0.1";
        bool isLogined = false;
        private string myid = "";
        private string username = "";
        private int pcno = 0;
        Thread ctThr;
        //int timeLeft = 0;
        public ClientForm()
        {
            InitializeComponent();
            this.panel2.Visible = false;

            // setting ip address
            System.IO.StreamReader file = new System.IO.StreamReader(".\\config");
            string line;
            char delimiter = ':';
            string ipaddr = "";
            int port = 0;

            while ((line = file.ReadLine()) != null)
            {
                String[] substrings = line.Split(delimiter);

                if (substrings[0] == "addr")
                {
                    ipaddr = substrings[1];    
                }
                if( substrings[0] == "port")
                {
                    port = Convert.ToInt32(substrings[1]);
                }
                if (substrings[0] == "pcno")
                {
                    pcno = Convert.ToInt32(substrings[1]);
                }
            }
            file.Close();
            
            IPEndPoint clientAddress = new IPEndPoint(IPAddress.Parse(ipaddr) , port);
            clientSocket = new TcpClient(clientAddress);

            // setting timer
            //timeLeft = 100;
            this.timer1.Interval = 1000;
            timer1.Tick += new EventHandler(timer_Tick);

            InitSocket();
            // start Thread
            ctThr = new Thread(GetPacket);
            ctThr.Start();

            //new Thread(delegate()
            //{
            //    
            //}).Start(GetPacket);
           
        }

        private void setLog(string msg)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                logbox.AppendText(msg + "\n");
            });
        }
        
        private void disconnect()
        {
            if (!clientSocket.Client.Connected) return;
                
            NetworkStream stream = clientSocket.GetStream();

            // 2. send the packet
            //Packet.Serialize(memberRegister).CopyTo(buffer, 0);
            byte[] sbuffer = Encoding.Unicode.GetBytes("dissconnect");
            stream.Write(sbuffer, 0, sbuffer.Length);
            stream.Flush();
         

            // 3. receive the packet
            //Array.Clear(buffer, 0, buffer.Length);

            //int bytesRead = stream.Read(buffer, 0, buffer.Length);
            //MemberRegisterResult mrResult = (MemberRegisterResult)Packet.Deserialize(buffer);

        }
        private void GetPacket()
        {
            while (true)
            {
                NetworkStream stream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                buffSize = clientSocket.ReceiveBufferSize;
                stream.Read(inStream, 0, buffSize);
                AnalyzePacket(inStream);
                //stream.Read(inStream, 0, buffSize);
                //string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            }

        }

        private void AnalyzePacket(byte[] buffer)
        {
            Packet packet = (Packet)Packet.Deserialize(buffer);

            if (packet == null)
                return;

            switch ((int)packet.packet_Type)
            {
                case (int)PacketType.Login_RESULT:
                    // 3. receive the packet
                //int bytesRead = stream.Read(buffer, 0, buffer.Length);
                LoginResult loginResult = (LoginResult)Packet.Deserialize(buffer);

                if (loginResult.result)
                {
                    myid = loginResult.reason;
                    //MessageBox.Show(loginResult.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    setLog("login success");
                    isLogined = true;
                    ChangePanel(isLogined);
                    GetUserTime();
                    StartTimer();
                }
                else
                {
                    setLog("login failed" + loginResult.reason);
                    MessageBox.Show(loginResult.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                break;

                case (int)PacketType.Member_REGISTER_RESULT:
                MemberRegisterResult mrResult = (MemberRegisterResult)Packet.Deserialize(buffer);

                if (mrResult.result)
                {
                    setLog("succeed register");// MessageBox.Show(mrResult.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    setLog("failed register");
                    //MessageBox.Show(mrResult.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                break;


                case (int)PacketType.GetTime:// get time

                    GetTime time = (GetTime)Packet.Deserialize(buffer);
                
                    char delimiter = ':';
                    String[] substrings = time.timeleft.Split(delimiter);
                    TimeLeft.hour = Convert.ToInt32(substrings[0]);
                    TimeLeft.min = Convert.ToInt32(substrings[1]);
                    TimeLeft.sec = Convert.ToInt32(substrings[2]);
                    TimeLeft.allTimeValue = TimeLeft.hour * 3600 + TimeLeft.min * 60 + TimeLeft.sec;
                break;
            }
            //stream.Close();
        }


        private void InitSocket()
        {
            try
            {
                clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"),9999));

                NetworkStream stream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(pcno + "$");
                stream.Write(outStream, 0, outStream.Length);
                stream.Flush();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);//MessageBox.Show(se.Message, "123Socket Error");
                this.Close();
                //clientSocket.Close();
                //clientSocket = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
               
            }

        }
        
        private void GetUserTime()
        {
            try
            {
                byte[] buffer = new byte[1024 * 4];

                // 1. connect to server
                //TcpClient client = new TcpClient("10.63.60.79", 9999);

                if (!clientSocket.Client.Connected)
                    clientSocket.Connect(serverAddr, 9999);

                NetworkStream stream = clientSocket.GetStream();

                // 2. send the packet
                GetTime gt = new GetTime();
                gt.packet_Type = (int)PacketType.GetTime;
                gt.id_str = myid;
                gt.timeleft = "";

                Packet.Serialize(gt).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);
               //  receive the packet
                Array.Clear(buffer, 0, buffer.Length);

                //int bytesRead = stream.Read(buffer, 0, buffer.Length);
                //GetTime time = (GetTime)Packet.Deserialize(buffer);
                
                //char delimiter = ':';
                //String[] substrings = time.timeleft.Split(delimiter);
                //TimeLeft.hour = Convert.ToInt32(substrings[0]);
                //TimeLeft.min = Convert.ToInt32(substrings[1]);
                //TimeLeft.sec = Convert.ToInt32(substrings[2]);
                //TimeLeft.allTimeValue = TimeLeft.hour * 3600 + TimeLeft.min * 60 + TimeLeft.sec;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); //MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartTimer()
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                timer1.Start();
            });
           
        }
        private void updateTimerLabel()
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                timelabel.Text = TimeLeft.hour + ":" + TimeLeft.min + ":" + TimeLeft.sec;
            });
            
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            updateTimerLabel(); 
           //timelabel.Text = TimeLeft.hour + ":" + TimeLeft.min + ":" + TimeLeft.sec;

            if(TimeLeft.sec ==0)
            {
                if (TimeLeft.min == 0)
                {
                    if(TimeLeft.hour ==0)
                    {
                        //end
                    }
                    else
                    {
                        TimeLeft.hour--;
                        TimeLeft.min = 59;
                        timer1.Stop();
                        timelabel.Text = "Time's up!";
                    }
                }
                else
                {
                    TimeLeft.min--;
                    TimeLeft.sec = 59;
                }

            }
            else
            {
                TimeLeft.sec--;
            }
            //timelabel.Text = DateTime.Now.ToLongTimeString();
        }

        //private void ChangePanel(bool bLogined)
        //{
        //    if (bLogined)
        //    {
        //        this.panel1.Visible = false;
        //        this.panel2.Visible = bLogined;
        //    }
        //    else
        //    {
        //        this.panel1.Visible = true;
        //        this.panel2.Visible = bLogined;
        //    }

        //}

        private void ChangePanel(bool bLogined)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                if (bLogined)
                {
                    this.panel1.Visible = false;
                    this.panel2.Visible = bLogined;
                }
                else
                {
                    this.panel1.Visible = true;
                    this.panel2.Visible = bLogined;
                }
            });
        }

        //login
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] buffer = new byte[1024 * 4];

                // 1. connect to server
                //TcpClient client = new TcpClient("10.63.60.79", 9999);

                if (!clientSocket.Client.Connected)
                    clientSocket.Connect(serverAddr, 9999);

                NetworkStream stream = clientSocket.GetStream();
                

                // 2. send the packet
                Login login = new Login();
                login.packet_Type = (int)PacketType.Login;
                login.id_str = tbUserId.Text.Trim();
                login.pw_str = tbUserPw.Text.Trim();
                //login.pcno_str = pcno.ToString();

                Packet.Serialize(login).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);

                // 3. receive the packet
                Array.Clear(buffer, 0, buffer.Length);

                //int bytesRead = stream.Read(buffer, 0, buffer.Length);
                //LoginResult loginResult = (LoginResult)Packet.Deserialize(buffer);

                //if (loginResult.result)
                //{
                //    myid = loginResult.reason;
                //    //MessageBox.Show(loginResult.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    setLog("login success");
                //    isLogined = true;
                //    ChangePanel(isLogined);
                //    GetUserTime();
                //    StartTimer();
                //}
                //else
                //{
                //    setLog("login failed" + loginResult.reason);
                //    MessageBox.Show(loginResult.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}

                // 4. close the socket
                //stream.Close();
                //clientSocket.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message); //MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            { 
                byte[] buffer = new byte[1024 * 4];

                // 1. connect to server
                //TcpClient client = new TcpClient("10.63.60.79", 9999);
                if (!clientSocket.Client.Connected)
                    clientSocket.Connect(serverAddr, 9999);
                NetworkStream stream = clientSocket.GetStream();

                // 2. send the packet
                MemberRegister memberRegister = new MemberRegister();
                memberRegister.packet_Type = (int)PacketType.Member_REGISTER;
                memberRegister.id_str = tbNewUserId.Text.Trim();
                memberRegister.pw_str = tbNewUserPw.Text.Trim();
                //memberRegister.nickname_str = textBox5.Text.Trim();

                Packet.Serialize(memberRegister).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);

                // 3. receive the packet
                Array.Clear(buffer, 0, buffer.Length);

                //int bytesRead = stream.Read(buffer, 0, buffer.Length);
                //MemberRegisterResult mrResult = (MemberRegisterResult)Packet.Deserialize(buffer);

                //if (mrResult.result)
                //{
                //    setLog("succeed register");// MessageBox.Show(mrResult.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                //else
                //{
                //    setLog("failed register");
                //    //MessageBox.Show(mrResult.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}

                // 4. close the socket
                //stream.Close();
                //client.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message); //MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timer1.Stop();
            ctThr.Abort();
            if (isLogined)
                SaveTime();

            if (clientSocket != null)
            {
                //disconnect();
                //clientSocket.Client.Disconnect(false);
                if (clientSocket.Connected)
                {
                    clientSocket.Client.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                clientSocket = null;
                
            }
          
            //SaveTime();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
           isLogined = false;
           ChangePanel(isLogined);
           this.timer1.Stop();
           SaveTime();
        }

        private void SaveTime()
        {
            byte[] buffer = new byte[1024 * 4];

            // 1. connect to server
            //TcpClient client = new TcpClient("10.63.60.79", 9999);
            //if (!clientSocket.Client.Connected)
            //    clientSocket.Connect(serverAddr, 9999);
            NetworkStream stream = clientSocket.GetStream();

            // 2. send the packet
            Logout logout = new Logout();
            logout.packet_Type = (int)PacketType.Logout;
            logout.id_str = myid;
            logout.timeleft = string.Format("{0}:{1}:{2}", TimeLeft.hour, TimeLeft.min, TimeLeft.sec);
            //memberRegister.nickname_str = textBox5.Text.Trim();
            Packet.Serialize(logout).CopyTo(buffer, 0);
            stream.Write(buffer, 0, buffer.Length);

            Array.Clear(buffer, 0, buffer.Length);

            //int bytesRead = stream.Read(buffer, 0, buffer.Length);
            //LogoutResult loResult = (LogoutResult)Packet.Deserialize(buffer);
            //stream.Close();
        }
    }
}


//private void getMessage()
//        {
//            while (true)
//            {
//                serverStream = clientSocket.GetStream();
//                int buffSize = 0;
//                byte[] inStream = new byte[10025];
//                buffSize = clientSocket.ReceiveBufferSize;
//                serverStream.Read(inStream, 0, buffSize);
//                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
//                readData = "" + returndata;
//                msg();
//            }
//        }

//        private void msg()
//        {
//            if (this.InvokeRequired)
//                this.Invoke(new MethodInvoker(msg));
//            else
//                textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + readData;
//        } 
