using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace TCPConnection
{
    public partial class ServerForm : Form
    {
        TcpListener server = null;
        TcpClient client = null;
        static int counter = 0; // client number
        private string strConnection = "server=localhost;Database=mydb;uid=admin;pwd=1234;";
        private MySqlConnection DBConnection = null;
        public ServerForm()
        {
            InitializeComponent();
            try
            {
                DBConnection = new MySqlConnection(strConnection);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            
            //InsertNeUser("asdf","asdf");

            // start socket
            Thread t = new Thread(InitSocket);
            t.IsBackground = true;
            t.Start();

            
        }
        // insert data into db
        private void InsertNeUser(string name, string password)
        {
            try
            {
                DBConnection.Open();
                string strQuery = string.Format("INSERT INTO user (_id, name, password) VALUES(NULL,'{0}',PASSWORD('{1}')); ",name,password);
                MySqlCommand cmd = new MySqlCommand(strQuery, DBConnection);
                int a = cmd.ExecuteNonQuery();
                DBConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void InitSocket()
        {
            server = new TcpListener(IPAddress.Any, 9999);
            client = default(TcpClient);
            server.Start();
            DisplayText(">> Server Started");

            while (true)
            {
                try
                {
                    counter++;
                    client = server.AcceptTcpClient();
                    DisplayText(">> Accept connection from client");

                    HandleClient hClient = new HandleClient();
                    hClient.OnReceived += new HandleClient.MessageDisplayHandler(DisplayText);
                    //hClient.OnReceived += new HandleClient.MessageDisplayHandler(GetPacketFromClient);
                    hClient.OnCalculated += new HandleClient.CalculateClientCounter(CalculateCounter);
                    hClient.StartClient(client, counter);

                }
                catch (SocketException se)
                {
                    Trace.WriteLine(string.Format("InitSocket - SocketException : {0}", se.Message));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("InitSocket - Exception :{0}", ex.Message));

                }
            }
        }
        
        private void CalculateCounter()
        {
            counter--;
        }
        private void GetPacketFromClient(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText(text + Environment.NewLine);
                }));
            }
            else
                richTextBox1.AppendText(text + Environment.NewLine);
        }

        private void DisplayText(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new MethodInvoker(delegate
                    {
                        richTextBox1.AppendText(text + Environment.NewLine);
                    }));
            }
            else
                richTextBox1.AppendText(text + Environment.NewLine);
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(client != null)
            {
                client.Close();
                client = null;
            }

            if (server != null)
            {
                server.Stop();
                server = null;
            }

            if (DBConnection != null)
            {
                DBConnection.Close();
                DBConnection = null;
            }
        }
    }
}
