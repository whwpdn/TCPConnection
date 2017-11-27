using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace TCPConnection
{
    public delegate void SendNewUserToServer();
    public partial class ClientForm : Form
    {
        TcpClient clientSocket = new TcpClient();

        public ClientForm()
        {
            InitializeComponent();
            new Thread(delegate()
            {
                InitSocket();
            }).Start();
        }

        private void InitSocket()
        {
            try
            {
                clientSocket.Connect("10.63.60.79", 9999);
                DisplayText("Client Started");
                labelStatus.Text = "Client socket program - Server Connected";
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message, "Socket Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            //NetworkStream stream = clientSocket.GetStream();
            //byte[] sbuffer = Encoding.Unicode.GetBytes(richTextBox2.Text + "$");
            //stream.Write(sbuffer, 0, sbuffer.Length);
            //stream.Flush();

            //byte[] rbuffer = new byte[1024];
            //stream.Read(rbuffer, 0, rbuffer.Length);
            //string msg = Encoding.Unicode.GetString(rbuffer);
            //msg = "Data from Server : " + msg;
            //DisplayText(msg);

            SendDataToServer(richTextBox2.Text + "$");
            GetDataFromServer();
            richTextBox2.Text = "";
            richTextBox2.Focus();
        }

        private void SendDataToServer(string data)
        {
            NetworkStream stream = clientSocket.GetStream();
            byte[] sbuffer = Encoding.Unicode.GetBytes(data);
            stream.Write(sbuffer, 0, sbuffer.Length);
            stream.Flush();

            //try
            //{
            //    byte[] buffer = new byte[1024 * 4];
            //    //TcpClient client = new TcpClient("127.0.0.1", 9999);
            //    NetworkStream stream = clientSocket.GetStream();

            //    // send the packet
            //    LoginPacket login = new LoginPacket();
            //    login.packetType = (int)PacketType.Login;
            //    login.id_str = "test";
            //    login.pw_str = "test2";

            //    Packet.Serialize(login).CopyTo(buffer, 0);

            //    stream.Write(buffer, 0, buffer.Length);

            //    // receive the packet
            //    Array.Clear(buffer, 0, buffer.Length);
            //    int bytesRead = stream.Read(buffer, 0, buffer.Length);
            //    LoginResult loginResult = (LoginResult)Packet.Deserialize(buffer);

            //    if (loginResult.result)
            //    {
            //        //ok
            //    }
            //    else
            //    {
            //        // error
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // exception
            //}
        }

        private void GetDataFromServer()
        {
            NetworkStream stream = clientSocket.GetStream();
            byte[] rbuffer = new byte[1024];
            stream.Read(rbuffer, 0, rbuffer.Length);
            string msg = Encoding.Unicode.GetString(rbuffer);
            msg = "Data from Server : " + msg;
            DisplayText(msg);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clientSocket != null)
                clientSocket.Close();
        }

        private void DisplayText(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText(Environment.NewLine + " >> " + text);
                }));
            }
            else
                richTextBox1.AppendText(Environment.NewLine + " >> " + text);
        }

        private void SendNewUserToServer()
        {
            
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            JoinForm join = new JoinForm();
            join.sendnewuserdata = new SendNewUserToServer(this.SendNewUserToServer);
            if (join.ShowDialog() == DialogResult.OK)
            {
                UserData newUser = join.GetUserData();

            }
        }
    }
}
