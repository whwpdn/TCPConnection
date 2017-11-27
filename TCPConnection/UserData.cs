using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnection
{
    public class UserData
    {
        string name;
        string password;
        string phone;
        string email;
        
        public UserData(string name, string password, string email, string phone )
        {
            this.name = name;
            this.password = password;
            this.phone = phone;
            this.email = email;      
        }

        public string UserName
        {
            get;
            set;
        }
        public string UserPassword
        {
            get;
            set;
        }
        public string UserPhone
        {
            get;
            set;
        }
        public string UserEmail
        {
            get;
            set;
        }
    }
}
