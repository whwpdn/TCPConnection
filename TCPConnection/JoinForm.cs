using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPConnection
{
    public partial class JoinForm : Form
    {
        UserData user;
        private SendNewUserToServer SendNewUserData;
        public SendNewUserToServer sendnewuserdata
        {
            get { return SendNewUserData; }
            set { SendNewUserData = value; }
        }
        public JoinForm()
        {
            InitializeComponent();
        }
        //public UserData Getdata()
        //{
        //    return user;
        //}
        public UserData GetUserData()
        {
            return user;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            user = new UserData(tbName.Text , tbPassword.Text , tbEmail.Text , tbPhone.Text);
            //SendNewUserData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
