using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace TcpServerForm
{
    public partial class PrepaidForm : Form
    {
        private MySqlConnection DBConnection = null;

        public string username { get; set; }
        public string time { get; set; }

        public PrepaidForm(MySqlConnection conn)
        {
            this.DBConnection = conn;
            InitializeComponent();
            //this.listView1.View = View.Details;
            //this.listView1.GridLines = true;
            //this.listView1.FullRowSelect = true;
            this.comboBox1.SelectedIndex = 0;
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            string username = this.tbUsername.Text;
            this.listView1.Items.Clear();
            try
            {
                DBConnection.Open();
                string strQuery = string.Format("SELECT _id,userid FROM user WHERE userid = '{0}';", username);
                MySqlCommand cmd = new MySqlCommand(strQuery, DBConnection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    ListViewItem newItem = new ListViewItem(new String[] { dataReader["_id"].ToString(), dataReader["userid"].ToString() });
                    this.listView1.Items.Add(newItem);
                    //time = dataReader["_id"].ToString();
                    //time = dataReader["name"].ToString();
                    
                }
                else
                {
                }
                dataReader.Close();
            }
            catch (MySqlException ee1)
            {
                //setLog(e.StackTrace);
                Console.WriteLine("Search click  : " + ee1.StackTrace);
                Console.WriteLine("Search click:" + ee1.Message);
            }
            finally
            {
                DBConnection.Close();
            }
            
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string userID = this.lbUserID.Text;
            if (userID == "")
                return;
            //string prepaidtime = string.Format("{0}:00:00",comboBox1.SelectedItem.ToString());
            string prepaidtime = comboBox1.SelectedItem.ToString();
            try
            {
                DBConnection.Open();
                //string strQuery = string.Format("UPDATE time SET timeleft = DATE_ADD(timeleft + INTERVAL '{0}' HOUR) WHERE _id = '{1}';", prepaidtime, userID);
                string strQuery = string.Format("UPDATE time SET timeleft = timeleft + INTERVAL '{0}' HOUR WHERE _id = '{1}';", prepaidtime, userID);
                MySqlCommand cmd = new MySqlCommand(strQuery, DBConnection);
                cmd.ExecuteReader();
                this.DialogResult = DialogResult.OK;
                this.username = this.lbSelected.Text;
                this.time =prepaidtime ;
            }
            catch (MySqlException ee1)
            {
                //setLog(e.StackTrace);
                Console.WriteLine("prepaid  : " + ee1.StackTrace);
                Console.WriteLine("prepaid :" + ee1.Message);
                this.DialogResult = DialogResult.Abort;
                this.username = "";
                this.time = "";

            }
            finally
            {
                DBConnection.Close();
            }
            
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ListView.SelectedListViewItemCollection items = this.listView1.SelectedItems;
            ListViewItem lvItem = items[0];
            //string name = lvItem.SubItems[1].Text;
            //string id = lvItem.SubItems[0].Text;
            //lbSelected.Text = name;
            //lbUserID.Text = id;

            lbUserID.Text = lvItem.SubItems[0].Text;
            lbSelected.Text = lvItem.SubItems[1].Text;
            

        }
    }
}
