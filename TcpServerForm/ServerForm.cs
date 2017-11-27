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
using System.IO;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Collections;
using MyPackets;
using MyUtils;


namespace TcpServerForm
{
    public partial class ServerForm : Form
    {
        public static Hashtable clientsList = new Hashtable();

        TcpListener listener = null;
        TcpClient client = null;
        static int counter = 0; // client number
        private MySqlConnection DBConnection = null;
        private const string strConnection = "server=localhost;Database=mydb;uid=admin;pwd=1234;";
        //Random random = new Random();b
        //Thread t_handler = null;

        private void setLog(string msg)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                textBox1.AppendText(msg + "\n");
            });
        }
        
        public ServerForm()
        {
            InitializeComponent();
            try
            {
                DBConnection = new MySqlConnection(strConnection);
            }
            catch (MySqlException e)
            {
                Console.WriteLine("DBConnection1 : " + e.StackTrace);
                Console.WriteLine("DBConnection2 : " + e.Message);
            }

            Thread t_handler = new Thread(StartSocket);
            t_handler.IsBackground = true;
            t_handler.Start();
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            
        }

        private void StartSocket()
        {
            // Buffer for reading data
            //byte[] buffer = new byte[1024 * 4];
            //string data = string.Empty;

            try
            {
                listener = new TcpListener(IPAddress.Any, 9999);
                listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                client = default(TcpClient);
                listener.Start();
                setLog("Server Started");
                        
                while (true)
                {

                    counter++;
                    client = listener.AcceptTcpClient();
                    string ipaddr = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                    NetworkStream stream = client.GetStream();
                    byte[] buff = new byte[10];
                    int iBytes = stream.Read(buff, 0, buff.Length);
                    string msg = Encoding.Unicode.GetString(buff, 0, iBytes);

                    //tempPacket = msg.Substring(iLastIndex, msg.IndexOf("$", iLastIndex));
                    int iLastIndex = msg.IndexOf("$");
                    if (iLastIndex > 0)
                    {
                        msg = msg.Substring(0, msg.IndexOf("$"));
                        clientsList.Add(msg, client);
                    }

                    AddClientToList(ipaddr , counter.ToString());

                    HandleClient hClient = new HandleClient();
                    hClient.OnPacketReceived += new HandleClient.AnalyzePacket(AnalyzePacket);
                    hClient.OnCalculated += new HandleClient.CalculateClientCounter(CalculateCounter);
                    hClient.StartClient(client, counter);
                    
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.StackTrace);
                Console.WriteLine("StartSocket2 : " + ex.Message);
                Console.WriteLine("StartSocket1 : " + ex.StackTrace);
            }
            finally
            {
                //client.Close();
            }
        }

        private void AddClientToList(string address , string key)
        {
            //ListViewItem newItem = new ListViewItem(new String[] { address});
            //newItem.Name = "1";
            //lvClient.Items.Add(newItem);
           // AddItemlist((Control)lvClient, address);
            setLog(address + " Connected!");
            AddListitem(lvClient, address, key);
        }
        private void AddUserToList(string userid, string username)
        {
            AddListitem(lvUser, username , userid );  
        }

        delegate void DaddListItem(ListView lv , string data, string key);
 

        private void AddListitem( ListView lv ,string data , string key)
        {
            if (lv.InvokeRequired)
            {
                DaddListItem call = new DaddListItem(AddListitem);
                this.Invoke(call,lv, data,key);
            }
            else
            {
                //ListViewItem newItem = new ListViewItem(new String[] { data });
                lv.Items.Add(key, data,0);
            }
        }
        delegate void DremoveListItem(ListView lv, string key);


        private void RemoveListItem(ListView lv, string key)
        {
            if (lv.InvokeRequired)
            {
                DremoveListItem call = new DremoveListItem(RemoveListItem);
                this.Invoke(call, lv, key);
            }
            else
            {
                lv.Items.RemoveByKey(key);
            }
        }
        private void RemoveClientItem(string address)
        {
            //ListViewItem[] newItem = this.lvClient.Items.Find(address, false);
        }
        private void RemoveUserItem(string userid)
        {

        }

        private void CalculateCounter(string address, int no)
        {
            setLog(address+ ": Disconnected");

            //this.lvClient.Items.RemoveByKey(address);
           // ListViewItem[] newItem = this.lvClient.Items.Find(address, false);
           // setLog(newItem[0].Name);
            //this.lvClient.Items.RemoveByKey("1");
            RemoveListItem(lvClient, no.ToString());
            client.Close();
            counter--;
        }
        private void SaveUserTimeLeft(string _id,string timeleft)
        {
            try
            {
                DBConnection.Open();
                string strQuery = string.Format("UPDATE time SET timeleft = '{0}' WHERE _id = '{1}';",timeleft, _id);
                MySqlCommand cmd = new MySqlCommand(strQuery, DBConnection);
                cmd.ExecuteReader();
            }
            catch (MySqlException e)
            {
                //setLog(e.StackTrace);
                Console.WriteLine("Save time  : " + e.StackTrace);
                Console.WriteLine("Save time:" + e.Message);
            }
            finally
            {
                DBConnection.Close();
            }
       }
        
        private string GetTimeLeft(string id)
        {
            string time = "";
            try
            {
                DBConnection.Open();
                string strQuery = string.Format("SELECT timeleft FROM time WHERE _id = '{0}';", id );
                MySqlCommand cmd = new MySqlCommand(strQuery, DBConnection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //MySqlDataAdapter da = new MySqlDataAdapter(strQuery, DBConnection);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                //foreach (DataRow row in ds.Tables[0].Rows)
                //{
                //    string stest = row["_id"].ToString();

                //}
                
                if (dataReader.Read())
                {
                      time = dataReader["timeleft"].ToString();
                }
                else
                {
                }
                dataReader.Close();
            }
            catch (MySqlException e)
            {
                //setLog(e.StackTrace);
                Console.WriteLine("GetTime  : " + e.StackTrace);
                Console.WriteLine("GetTime :" + e.Message);
            }
            finally
            {
                DBConnection.Close();
            }
            return time;
          
        }

        private void AnalyzePacket(NetworkStream stream, byte[] buffer)
        {
            Packet packet = (Packet)Packet.Deserialize(buffer);

            if (packet == null)
                return;

            switch ((int)packet.packet_Type)
            {
                case (int)PacketType.Login:
                    {
                        // 받은 패킷을 Login class 로 deserialize 시킴
                        Login login = (Login)Packet.Deserialize(buffer);

                        setLog(string.Format("ID : {0}, PWD : {1}", login.id_str, login.pw_str));

                        // 전송할 패킷을 LoginResult class 로 serialize 시킴
                        LoginResult loginResult = new LoginResult();
                        loginResult.packet_Type = (int)PacketType.Login_RESULT;
                        string id = CheckLogin(login.id_str, login.pw_str);
                        if(id.Length !=0)
                        {

                            if (Utils.CheckTimeleft(GetTimeLeft(id)))
                            {
                                setLog(string.Format("Login : {0} ", login.id_str));
                                loginResult.result = true;
                                loginResult.reason = id;
                                AddUserToList(id, login.id_str);

                                
                            }
                            else
                            {
                                loginResult.result = false;
                                setLog("failed Login");
                                loginResult.reason = "시간을 충전하세요";
                            } 
                        }
                        else
                        {
                            loginResult.result = false;
                            setLog("failed Login");
                            loginResult.reason = "아이디와 비밀번호를 확인 하시기 바랍니다.";
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(loginResult).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);

                        setLog("");
                    }
                    break;
                case (int)PacketType.Member_REGISTER:
                    {
                        // 받은 패킷을 MemberRegister class 로 deserialize 시킴
                        MemberRegister memberRegister = (MemberRegister)Packet.Deserialize(buffer);

                        setLog(string.Format("ID : {0}, PWD : {1}", 
                                                memberRegister.id_str, memberRegister.pw_str));

                        // 전송할 패킷을 LoginResult class 로 serialize 시킴
                        MemberRegisterResult mrResult = new MemberRegisterResult();
                        mrResult.packet_Type = (int)PacketType.Member_REGISTER_RESULT;
                        if(InsertNeUser(memberRegister.id_str, memberRegister.pw_str))
                        {
                            mrResult.result = true;
                            mrResult.reason = "회원 가입이 정상적으로 되었습니다.";
                            setLog(string.Format("succeed register : {0}", memberRegister.id_str));
                        }
                        else
                        {
                            mrResult.result = false;
                            mrResult.reason = "회원가입 오류입니다.";
                            setLog("failed register");
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(mrResult).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);

                        setLog("");
                    }
                    break;

                case (int)PacketType.GetTime:// get time
                    // 받은 패킷을 MemberRegister class 로 deserialize 시킴
                    GetTime gt= (GetTime)Packet.Deserialize(buffer);

                    setLog(string.Format("request timeleft.{0}-{1}",
                                            gt.id_str, gt.timeleft));
                    gt.timeleft = GetTimeLeft(gt.id_str);

                    Array.Clear(buffer, 0, buffer.Length);
                    Packet.Serialize(gt).CopyTo(buffer, 0);
                    stream.Write(buffer, 0, buffer.Length);

                    setLog("");
                    break;   
                case (int)PacketType.Logout:
                    // 받은 패킷을 MemberRegister class 로 deserialize 시킴
                    Logout lo = (Logout)Packet.Deserialize(buffer);

                    setLog(string.Format("Logout:{0}- save tileleft {1}",
                                            lo.id_str, lo.timeleft));

                    SaveUserTimeLeft(lo.id_str, lo.timeleft);
                    RemoveListItem(lvUser, lo.id_str);
                    Array.Clear(buffer, 0, buffer.Length);
                    
                    //LogoutResult lr = new LogoutResult();
                    //lr.packet_Type = (int)PacketType.Logout_Result;
                    //lr.result = true;
                    //lr.reason = "normal";
                    //Packet.Serialize(lr).CopyTo(buffer, 0);
                    //stream.Write(buffer, 0, buffer.Length);
                    break;

            }
            //stream.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listener != null)
            {
                listener.Stop();
            }
            this.Close();
        }

        // insert data into db
        private bool InsertNeUser(string userid, string password)
        {
            bool result = false;
            try
            {
                DBConnection.Open();
                string strQuery = string.Format("INSERT INTO user (_id, userid, password) VALUES(NULL,'{0}',PASSWORD('{1}')); " +
                                                "INSERT INTO time (_id) VALUES(LAST_INSERT_ID());", userid, password);
                MySqlCommand cmd = new MySqlCommand(strQuery, DBConnection);
                int a = cmd.ExecuteNonQuery();
                if (a> 0)
                {
                    result = true;
                }
                //DBConnection.Close();
                
            }
            catch (Exception e)
            {      
                //Console.WriteLine("insert newUser1 : " + e.StackTrace);
                Console.WriteLine("insert newUser2 : " + e.Message);

                result = false;
            }
            finally
            {
                if(DBConnection != null)
                 DBConnection.Close();
            }
            return result;
        }

        private string CheckLogin(string userid, string password)
        {
            //bool result = false;
            string _id = string.Empty;

            try
            {
                DBConnection.Open();
                string strQuery = string.Format("SELECT _id FROM user WHERE userid='{0}' AND password=PASSWORD('{1}');", userid, password);
                MySqlCommand cmd = new MySqlCommand(strQuery, DBConnection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //MySqlDataAdapter da = new MySqlDataAdapter(strQuery, DBConnection);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                //foreach (DataRow row in ds.Tables[0].Rows)
                //{
                //    string stest = row["_id"].ToString();
                    
                //}

                if(dataReader.Read())
                {
                    _id = dataReader["_id"].ToString();
                }
                else
                {
                }
                dataReader.Close();
            }
            catch (MySqlException e)
            {
                //setLog(e.StackTrace);
                Console.WriteLine("Login function1  : " + e.StackTrace);
                Console.WriteLine("Login function2 :" + e.Message);
            }
            finally
            {
                DBConnection.Close();
            }

            return _id;

        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (t_handler != null)
            //{
            //    t_handler.Abort();
            //}
           
            if (client != null)
            {
                client.Close();
                client = null;
            }
            if (DBConnection != null)
            {
                DBConnection.Close();
                DBConnection = null;
            }
            //try
            //{
            //    listener.Server.Shutdown(SocketShutdown.Both);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("formClosing" + ex);
            //}
            //finally
            //{
            //    listener.Server.Close(0);
            //    listener.Stop();
            //}

            if (listener != null)
            {
                if (listener.Server.Connected)
                {
                    listener.Server.Disconnect(false);
                }
                //listener.EndAcceptTcpClient();
                listener.Stop();
                listener = null;
            }

           
        }

        private void prepaidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrepaidForm ppf = new PrepaidForm(this.DBConnection);
            if (ppf.ShowDialog() == DialogResult.OK)
            {
                setLog(string.Format("{0} : {1} hour prepaid ", ppf.username, ppf.time));
            }
            else
                setLog("prepaid failed");
        }
        
        private void UpdateUserClientTime()
        {
            GetTime gt = new GetTime();
            byte[] buffer = new byte[1024];
            Array.Clear(buffer, 0, buffer.Length);
            Packet.Serialize(gt).CopyTo(buffer, 0);

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
